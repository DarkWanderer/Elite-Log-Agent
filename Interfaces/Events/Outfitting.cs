namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class Outfitting : LogEvent
    {
        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("Horizons")]
        public bool? Horizons { get; set; }

        [JsonProperty("Items")]
        public OutfittingItem[] Items { get; set; }
    }
}
