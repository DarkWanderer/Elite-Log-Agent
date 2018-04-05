using InaraUpdater.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater
{
    public class RestClient : IRestClient
    {
        private string url;
        private static HttpClient client = new HttpClient();

        public RestClient(string url)
        {
            this.url = url;
        }

        public async Task<string> PostAsync(string input)
        {
            var httpContent = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
