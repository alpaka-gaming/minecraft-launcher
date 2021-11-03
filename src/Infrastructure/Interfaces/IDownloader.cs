using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Models;
using Infrastructure.Services;

namespace Infrastructure.Interfaces
{
    public interface IDownloader
    {
        void EnableUserAuthorization();
        Task Download(string updateIdentity, string revisionNumber, string destination, Downloader.DownloadProgress progress, CancellationToken cancellationToken);
        
        
        Task<IEnumerable<Game>> GetVersionsAsync();
    }
}