using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DW.ELA.ErrorReportingApi.Controllers
{
    [Route("api/[controller]")]
    public partial class ErrorsController : Controller
    {
        private readonly ILogger<ErrorsController> logger;
        private readonly IConfiguration configuration;
        private readonly string baseDirectory;

        public ErrorsController(ILogger<ErrorsController> logger, IConfiguration configuration, IHostingEnvironment env)
        {
            this.logger = logger;
            this.configuration = configuration;
            baseDirectory = env.ContentRootPath;
        }

        // POST api/values
        [HttpGet]
        public string Get()
        {
            return "OK";
        }

        [HttpPost]
        public ActionResult Post([FromBody]ExceptionRecord[] body)
        {
            try
            {
                foreach (var record in body)
                {
                    var directory = Path.Combine(baseDirectory, "errors", record.SoftwareVersion);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    var json = JsonConvert.SerializeObject(record, Formatting.Indented);
                    var hash = Hash.Sha1(json);
                    
                    var fileName = Path.Combine(directory, hash + ".json");
                    if (!System.IO.File.Exists(fileName))
                        System.IO.File.WriteAllText(fileName, json);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while processing Post request");
                return StatusCode(500, e.Message);
            }

            return StatusCode(200);
        }

        [HttpDelete]
        public void Delete()
        {
            var directory = Path.Combine(baseDirectory, "errors");
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }

        [JsonObject("exceptionRecord")]
        public class ExceptionRecord
        {
            [JsonConstructor]
            public ExceptionRecord(string softwareVersion, string cmdrNameHash, string exceptionString)
            {
                SoftwareVersion = softwareVersion;
                CmdrNameHash = cmdrNameHash;
                ExceptionString = exceptionString;
            }

            [JsonProperty("version")]
            public string SoftwareVersion { get; }

            [JsonProperty("cmdrNameHash")]
            public string CmdrNameHash { get; }

            [JsonProperty("exceptionString")]
            public string ExceptionString { get; }
        }

        private static class Hash
        {
            public static string Sha1(string str)
            {
                using (var sha1 = new SHA1Managed())
                    return ByteArrayToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(str)));
            }

            private static string ByteArrayToString(byte[] bytes)
            {
                char[] c = new char[bytes.Length * 2];
                int b;
                for (int i = 0; i < bytes.Length; i++)
                {
                    b = bytes[i] >> 4;
                    c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                    b = bytes[i] & 0xF;
                    c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
                }
                return new string(c);
            }
        }
    }
}