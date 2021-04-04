using System.Net;
using System.Threading.Tasks;

namespace Updater
{
    public static class Extensions
    {
        public static async Task<long> GetLengthAsync(this string url)
        {
            long length = 0;
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
            request.Method = "HEAD";
            using (var response = await request.GetResponseAsync())
                length = response.ContentLength;

            return length;
        }
    }
}