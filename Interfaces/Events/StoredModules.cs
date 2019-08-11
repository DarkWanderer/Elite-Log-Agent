namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class StoredModules : JournalEvent
    {
        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("Items")]
        public StoredItem[] Items { get; set; }
    }
}
