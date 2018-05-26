using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Utility
{
    public class ThrottlingRestClient : IRestClient
    {
        private readonly string url;
        private static HttpClient client = new HttpClient();

        private DateTime lastRequestTimestamp = DateTime.MinValue;
        private int requestCounter;
        private readonly object @lock = new object();

        private void ThrowIfQuotaExceeded()
        {
            lock (@lock)
            {
                var now = DateTime.UtcNow;
                int secondsSinceLastCall = 0;

                if (lastRequestTimestamp != DateTime.MinValue)
                    secondsSinceLastCall = (int)(now - lastRequestTimestamp).TotalSeconds;

                int decayedRequestCounter = Math.Max(0,requestCounter - secondsSinceLastCall / 5);
                //if (decayedRequestCounter > 3)
                //    throw new ApplicationException("Internal error: queries are too frequent");

                lastRequestTimestamp = now;
                requestCounter = decayedRequestCounter + 1;
            }
        }

        public ThrottlingRestClient(string url)
        {
            this.url = url;
        }

        public async Task<string> PostAsync(string input)
        {
            ThrowIfQuotaExceeded();
            var httpContent = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostAsync(IDictionary<string,string> values)
        {
            ThrowIfQuotaExceeded();

            var encodedPost = string.Join("&", values.Select(kvp => kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value)));

            var httpContent = new StringContent(encodedPost, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync(url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAsync(string urlTail)
        {
            ThrowIfQuotaExceeded();
            var response = await client.GetAsync(url + urlTail);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
