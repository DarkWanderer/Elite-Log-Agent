namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class Market : LogEvent
    {
        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("Items")]
        public MarketItem[] Items { get; set; }
    }
}
