namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class StoredShips : LogEvent
    {
        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("ShipsHere")]
        public ShipHere[] ShipsHere { get; set; }

        [JsonProperty("ShipsRemote")]
        public ShipRemote[] ShipsRemote { get; set; }
    }

    public class ShipHere
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

    public class ShipRemote
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
        public long ShipMarketId { get; set; }

        [JsonProperty("TransferPrice")]
        public long TransferPrice { get; set; }

        [JsonProperty("TransferTime")]
        public long TransferTime { get; set; }

        [JsonProperty("Value")]
        public long Value { get; set; }

        [JsonProperty("Hot")]
        public bool Hot { get; set; }
    }
}
