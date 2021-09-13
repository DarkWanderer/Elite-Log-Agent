using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class Cargo : JournalEvent
    {
        [JsonProperty("Vessel")]
        public string Vessel { get; set; }

        [JsonProperty("Count")]
        public long? Count { get; set; }

        [JsonProperty("Inventory")]
        public InventoryRecord[] Inventory { get; set; }
    }
}
