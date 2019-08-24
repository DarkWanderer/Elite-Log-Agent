namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Shipyard : JournalEvent
    {
        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("Horizons")]
        public bool? Horizons { get; set; }

        [JsonProperty("AllowCobraMkIV")]
        public bool? AllowCobraMkIv { get; set; }

        [JsonProperty("PriceList")]
        public ShipyardPrice[] Prices { get; set; }
    }
}
