using System.Net;

namespace Mandro.Utils.Web
{
    public static class WebClientExtensions
    {
        public static string Get(this WebClient client, string url)
        {
            return client.DownloadString(url);
        }

        public static string Post(this WebClient client, string url, string data)
        {
            return client.UploadString(url, "POST", data);
        }

        public static string Delete(this WebClient client, string url, string data = null)
        {
            return client.UploadString(url, "DELETE", data ?? string.Empty);
        }

        public static string Put(this WebClient client, string url, string data)
        {
            return client.UploadString(url, "PUT", data);
        }
    }
}