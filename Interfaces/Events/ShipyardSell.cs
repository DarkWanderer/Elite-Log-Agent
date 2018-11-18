using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class ShipyardSell : LogEvent
    {
        [JsonProperty("ShipType")]
        public string ShipType { get; set; }

        [JsonProperty("ShipType_Localised")]
        public string ShipTypeLocalised { get; set; }

        [JsonProperty("SellShipID")]
        public long SellShipId { get; set; }

        [JsonProperty("ShipPrice")]
        public long ShipPrice { get; set; }

        [JsonProperty("MarketID")]
        public long? MarketId { get; set; }

        [JsonProperty("ShipMarketID")]
        public long? ShipMarketId { get; set; }

        [JsonProperty("System")]
        public string System { get; set; }
    }
}
