namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class StoredShips : JournalEvent
    {
        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("ShipsHere")]
        public ShipStoredLocal[] ShipsHere { get; set; }

        [JsonProperty("ShipsRemote")]
        public ShipStoredRemote[] ShipsRemote { get; set; }
    }
}
