using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class Undocked : LogEvent
    {
        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("StationType")]
        public string StationType { get; set; }

        [JsonProperty("MarketID")]
        public long MarketId { get; set; }
    }
}
