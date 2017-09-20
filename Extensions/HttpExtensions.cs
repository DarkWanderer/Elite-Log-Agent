using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DW.Inara.LogUploader.Extensions
{
    public static class HttpExtensions
    {
        public static HttpResponseMessage EnsureSuccessful(this HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.ReasonPhrase);
            if (response.Content.ReadAsStringAsync().Result.Contains("No mass site leeching"))
                throw new HttpRequestException("Leeching protection has been enabled on Inara for your IP. Wait 48 hours");
            return response;
        }
    }
}
