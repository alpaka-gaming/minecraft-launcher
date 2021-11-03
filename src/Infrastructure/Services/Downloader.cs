using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Flurl.Http;
using Infrastructure.Exceptions;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.References;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services
{
    public class Downloader : IDownloader
    {
        private HttpClient client = new HttpClient();
        private WUProtocol protocol = new WUProtocol();
        
        private readonly DownloaderOptions _options = new DownloaderOptions();
        private readonly ILogger _logger;

        public Downloader(IConfiguration configuration, LoggerFactory loggerFactory)
        {
            configuration.Bind("Services:Downloader", _options);
            _logger = loggerFactory.CreateLogger(GetType());
        }

        private async Task<XDocument> PostXmlAsync(string url, XDocument data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings {Indent = false, OmitXmlDeclaration = true}))
                {
                    data.Save(xmlWriter);
                }

                request.Content = new StringContent(stringWriter.ToString(), Encoding.UTF8, "application/soap+xml");
            }

            using (var resp = await client.SendAsync(request))
            {
                var str = await resp.Content.ReadAsStringAsync();
                return XDocument.Parse(str);
            }
        }

        private async Task DownloadFile(string url, string to, DownloadProgress progress, CancellationToken cancellationToken)
        {
            using (var resp = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                using (var inStream = await resp.Content.ReadAsStreamAsync())
                using (var outStream = new FileStream(to, FileMode.Create))
                {
                    var totalSize = resp.Content.Headers.ContentLength;
                    progress(0, totalSize);
                    long transferred = 0;
                    var buf = new byte[1024 * 1024];
                    while (true)
                    {
                        var n = await inStream.ReadAsync(buf, 0, buf.Length, cancellationToken);
                        if (n == 0)
                            break;
                        await outStream.WriteAsync(buf, 0, n, cancellationToken);
                        transferred += n;
                        progress(transferred, totalSize);
                    }
                }
            }
        }

        private async Task<string> GetDownloadUrl(string updateIdentity, string revisionNumber)
        {
            var result = await PostXmlAsync(protocol.GetDownloadUrl(),
                protocol.BuildDownloadRequest(updateIdentity, revisionNumber));
            Debug.WriteLine($"GetDownloadUrl() response for updateIdentity {updateIdentity}, revision {revisionNumber}:\n{result}");
            foreach (var s in protocol.ExtractDownloadResponseUrls(result))
            {
                if (s.StartsWith("http://tlu.dl.delivery.mp.microsoft.com/"))
                    return s;
            }

            return null;
        }

        public void EnableUserAuthorization()
        {
            protocol.SetMSAUserToken(WUTokenHelper.GetWUToken());
        }

        public async Task Download(string updateIdentity, string revisionNumber, string destination, DownloadProgress progress, CancellationToken cancellationToken)
        {
            var link = await GetDownloadUrl(updateIdentity, revisionNumber);
            if (link == null)
                throw new BadUpdateIdentityException();
            Debug.WriteLine("Resolved download link: " + link);
            await DownloadFile(link, destination, progress, cancellationToken);
        }

        public delegate void DownloadProgress(long current, long? total);

        public async Task<IEnumerable<Game>> GetVersionsAsync()
        {
            var fileName = "Versions.json";
            try
            {
                var host = _options.Versions;
                var data = await host.GetBytesAsync();
                if (data != null && data.Length > 0)
                    await File.WriteAllBytesAsync(fileName, data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            if (File.Exists(fileName))
            {
                var content = await File.ReadAllTextAsync(fileName);
                var result = JArray.Parse(content).AsEnumerable().Reverse().Select(o =>
                    new Game(o[0].Value<string>(), o[1].Value<string>(), o[2].Value<int>() == 1)
                );
                return result;
            }

            return null;
        }
    }
}