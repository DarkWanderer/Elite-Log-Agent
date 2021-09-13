using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class ShipyardTransfer : JournalEvent
    {
        [JsonProperty("ShipType")]
        public string ShipType { get; set; }

        [JsonProperty("ShipType_Localised")]
        public string ShipTypeLocalised { get; set; }

        [JsonProperty("ShipID")]
        public long ShipId { get; set; }

        [JsonProperty("System")]
        public string System { get; set; }

        [JsonProperty("ShipMarketID")]
        public long ShipMarketId { get; set; }

        [JsonProperty("Distance")]
        public double Distance { get; set; }

        [JsonProperty("TransferPrice")]
        public long TransferPrice { get; set; }

        [JsonProperty("TransferTime")]
        public long TransferTime { get; set; }

        [JsonProperty("MarketID")]
        public long MarketId { get; set; }
    }
}
