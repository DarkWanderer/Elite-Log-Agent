namespace DW.ELA.Interfaces
{
    using System;
    using Newtonsoft.Json;

    public class ErrorRecord
    {
        [JsonProperty("hostname")]
        public string Hostname;
        [JsonProperty("exceptionTypeName")]
        public string ExceptionTypeName;
        [JsonProperty("exceptionCallStack")]
        public string ExceptionCallStack;
        [JsonProperty("exceptionMessage")]
        public string ExceptionMessage;
        [JsonProperty("exceptionFullText")]
        public string ExceptionFullText;

        public static ErrorRecord FromException(Exception e)
        {
            var record = new ErrorRecord();
            return record;
        }
    }
}
