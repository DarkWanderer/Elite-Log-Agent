namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class ShipStoredLocal
    {
        [JsonProperty("ShipID")]
        public long ShipId { get; set; }

        [JsonProperty("ShipType")]
        public string ShipType { get; set; }

        [JsonProperty("ShipType_Localised", NullValueHandling = NullValueHandling.Ignore)]
        public string ShipTypeLocalised { get; set; }

        [JsonProperty("Value")]
        public long Value { get; set; }

        [JsonProperty("Hot")]
        public bool Hot { get; set; }

        [JsonProperty("Name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}
