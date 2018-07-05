using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public class Shipyard : LogEvent
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
        public Price[] Price { get; set; }
    }

    public class Price
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("ShipType")]
        public string ShipType { get; set; }

        [JsonProperty("ShipPrice")]
        public long ShipPrice { get; set; }

        [JsonProperty("ShipType_Localised", NullValueHandling = NullValueHandling.Ignore)]
        public string ShipTypeLocalised { get; set; }
    }
}
