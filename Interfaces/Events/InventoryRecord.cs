using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class InventoryRecord
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Name_Localised")]
        public string NameLocalised { get; set; }

        [JsonProperty("Count")]
        public long Count { get; set; }

        [JsonProperty("Stolen")]
        public long? Stolen { get; set; }
    }
}
