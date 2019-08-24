namespace DW.ELA.Interfaces
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class JournalEvent
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
