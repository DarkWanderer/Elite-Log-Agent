using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DW.ELA.Interfaces
{
    public class JournalEvent : IJournalEvent
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonIgnore]
        public JObject Raw { get; set; }

        public override string ToString() => Event;
    }
}
