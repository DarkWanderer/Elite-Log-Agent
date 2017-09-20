using DW.Inara.LogUploader.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DW.Inara.LogUploader.Inara
{
    internal static class Uploader
    {
        private static readonly Uri baseUri = new Uri("https://inara.cz/");
        private static readonly Uri uploadUri = new Uri("https://inara.cz/cmdr/");
        private static readonly Uri loginUri = new Uri("https://inara.cz/login/");

        // Define other methods and classes here
        public static void UploadFile(string filePath, string username, string password)
        {
            var cookies = GetCookies(username, password);

            using (var handler = new HttpClientHandler() { CookieContainer = cookies })
            using (var client = new HttpClient(handler))
            using (var readStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var formData = new MultipartFormDataContent())
            {
                HttpContent fileStreamContent = new StreamContent(readStream);
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                formData.Add(new StringContent("cmdr"), "location");
                formData.Add(new StringContent("JOURNAL_LOG_IMPORT"), "formact");
                formData.Add(fileStreamContent, "importfile[]", Path.GetFileName(filePath));

                var request = new HttpRequestMessage(HttpMethod.Post, uploadUri);
                request.Content = formData;
                request.Headers.Add("Origin", baseUri.ToString());
                request.Headers.Referrer = uploadUri;

                var response = client.SendAsync(request).Result.EnsureSuccessful();
            }
        }

        public static bool ValidateCredentials(string username, string password)
        {
            try
            {
                var cookies = GetCookies(username, password);
                return cookies != null;
            }catch
            {
                return false;
            }
        }

        private static CookieContainer GetCookies(string username, string password)
        {
            var cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(baseUri, "elitesheet=0");
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                NameValueCollection outgoingQueryString = System.Web.HttpUtility.ParseQueryString(String.Empty);
                outgoingQueryString.Add("loginid", username);
                outgoingQueryString.Add("loginpass", password);
                outgoingQueryString.Add("loginremember", "1");
                outgoingQueryString.Add("formact", "ENT_LOGIN");
                outgoingQueryString.Add("location", "intro");
                string postdata = outgoingQueryString.ToString();

                var request = new HttpRequestMessage(HttpMethod.Post, loginUri);
                request.Content = new StringContent(postdata);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Add("Origin", baseUri.ToString());
                request.Headers.Referrer = loginUri;

                var response = client.SendAsync(request).Result.EnsureSuccessful();

                var cookieStrings = response.Headers.GetValues("Set-Cookie").ToList();

                if (!cookieStrings.Any(s => s.Contains("elitesheet")))
                    throw new HttpRequestException("Failed to authenticate with Inara");

                var container = new CookieContainer();
                foreach (var cookieString in cookieStrings)
                    container.SetCookies(baseUri, cookieString);
                return container;
            }
        }
    }
}
