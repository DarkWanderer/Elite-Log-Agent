using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace DW.ELA.ErrorReportingApi.Controllers
{
    public partial class ErrorsController
    {
        public class ExceptionRecord
        {
            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("exceptionType")]
            public string ExceptionType { get; set; }

            [JsonProperty("callStack")]
            public string CallStack { get; set; }

            [JsonProperty("version")]
            public string SoftwareVersion { get; set; }
        }
    }
}