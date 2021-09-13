using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class ShipStoredRemote
    {
        [JsonProperty("ShipID")]
        public long ShipId { get; set; }

        [JsonProperty("ShipType")]
        public string ShipType { get; set; }

        [JsonProperty("ShipType_Localised")]
        public string ShipTypeLocalised { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("ShipMarketID")]
        public long? ShipMarketId { get; set; }

        [JsonProperty("TransferPrice")]
        public long? TransferPrice { get; set; }

        [JsonProperty("TransferTime")]
        public long? TransferTime { get; set; }

        [JsonProperty("Value")]
        public long Value { get; set; }

        [JsonProperty("Hot")]
        public bool Hot { get; set; }

        [JsonProperty]
        public bool? InTransit { get; set; }
    }
}
