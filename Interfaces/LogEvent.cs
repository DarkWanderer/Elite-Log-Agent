using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DW.ELA.LogModel
{
    public class LogEvent
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonIgnore]
        public JObject Raw { get; set; }
    }
}
