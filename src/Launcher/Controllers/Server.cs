using Flurl.Http;
using Launcher.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Launcher.Controllers
{

    public interface IServer
    {
        Dictionary<string, Version> Versions { get; set; }

        string Profile { get; }
        string ServerUrl { get; }

        Uri FeedUrl { get; }
        Uri FastDownload { get; }

        Task<bool> ConnectAsync();

        Task<List<string>> GetFiles(string folder);

        Task<bool> GetProfile();


    }
    internal class Server : IServer
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public Server(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _configuration = configuration;
        }

        public Dictionary<string, Version> Versions { get; set; }

        public string Profile => _configuration["AppSettings:Profile"];
        public string ServerUrl => new(_configuration["AppSettings:ServerUrl"]);

        public Uri FeedUrl => new(_configuration["AppSettings:FeedUrl"]);
        public Uri FastDownload => new(_configuration["AppSettings:FastDownloadUrl"]);

        private bool PingServer()
        {
            try
            {
                var ping = new Ping();
                var result = ping.Send(ServerUrl);
                return (result != null && result.Status == IPStatus.Success);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }
        private bool PingFastDownload()
        {
            try
            {
                var ping = new Ping();
                var result = ping.Send(FastDownload.Host);
                return (result != null && result.Status == IPStatus.Success);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<bool> ConnectAsync()
        {
            var attempts = 1;
            var isServerOn = false;
            var isFastDownloadOn = false;
            while (!isServerOn)
            {
                if (attempts >= 5) break;
                Console.Write("Conectando con el servidor:");
                isServerOn = PingServer();
                isFastDownloadOn = PingFastDownload();
                if (isServerOn && isFastDownloadOn)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" [LISTO]");
                    Console.WriteLine();
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" [FALLIDO]");
                    Console.WriteLine();
                    Console.ResetColor();
                    Console.WriteLine($"Reintentando en {attempts} segundos.");
                    await Task.Delay(attempts * 1000);
                }

                attempts++;
            }

            return isServerOn;
        }

        public async Task<List<string>> GetFiles(string folder)
        {
            var fixString = new Func<string, string>(m => { return HttpUtility.UrlDecode(m.Replace("+", "%2b")); });

            var url = $"{FastDownload}{folder}";

            var files = new List<string>();
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var html = reader.ReadToEnd();
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    var nodes = doc.DocumentNode.SelectNodes("html/body/pre/a");
                    var links = nodes.Select(m => m.Attributes.First(a => a.Name == "href").Value);
                    files.AddRange(links.Where(m => !m.StartsWith("?") && !m.StartsWith("/")).Select(m => fixString(m)));
                }
            }

            await Task.Yield();
            return files;
        }

        public async Task<bool> GetProfile()
        {
            Console.Write("Obteniendo perfil:");
            try
            {
                var tempFile = new FileInfo(Path.GetTempFileName());
                var tempPath = tempFile.Directory?.FullName;
                await $"{FastDownload}/{Profile}.json".DownloadFileAsync(tempPath, tempFile.Name);
                var data = await File.ReadAllTextAsync(tempFile.FullName);
                var definition = JsonConvert.DeserializeObject<JObject>(data);

                Versions = new Dictionary<string, Version>();
                foreach (var item in definition.Children())
                    Versions.Add(item.Path, new Version(item.First().ToString()));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" [LISTO]");
                Console.WriteLine();
                Console.ResetColor();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [ERROR]");
                Console.WriteLine();
                Console.ResetColor();

                return false;
            }
        }


    }
}
