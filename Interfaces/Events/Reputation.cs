namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class Reputation : LogEvent
    {
        [JsonProperty("Empire")]
        public double Empire { get; set; }

        [JsonProperty("Federation")]
        public double Federation { get; set; }

        [JsonProperty("Alliance")]
        public double Alliance { get; set; }

        [JsonProperty("Independent")]
        public double? Independent { get; set; }
    }
}
