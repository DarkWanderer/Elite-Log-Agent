using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public class StoredModules : LogEvent
    {
        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("Items")]
        public Item[] Items { get; set; }
    }

    public class Item
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Name_Localised")]
        public string NameLocalised { get; set; }

        [JsonProperty("StorageSlot")]
        public long StorageSlot { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("TransferCost")]
        public long TransferCost { get; set; }

        [JsonProperty("TransferTime")]
        public long TransferTime { get; set; }

        [JsonProperty("BuyPrice")]
        public long BuyPrice { get; set; }

        [JsonProperty("Hot")]
        public bool Hot { get; set; }

        [JsonProperty("EngineerModifications")]
        public string EngineerModifications { get; set; }

        [JsonProperty("Level")]
        public long? Level { get; set; }

        [JsonProperty("Quality")]
        public long? Quality { get; set; }
    }
}
