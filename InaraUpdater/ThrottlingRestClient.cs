using InaraUpdater.Model;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater
{
    public class ThrottlingRestClient : IRestClient
    {
        private readonly string url;
        private static HttpClient client = new HttpClient();
        private DateTime lastRequestTimestamp = DateTime.MinValue;

        public ThrottlingRestClient(string url)
        {
            this.url = url;
        }

        public async Task<string> PostAsync(string input)
        {
            var now = DateTime.UtcNow;
            if (now - lastRequestTimestamp < TimeSpan.FromSeconds(1))
                throw new ApplicationException("Internal error: queries are too frequent");
            lastRequestTimestamp = now;

            var httpContent = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
