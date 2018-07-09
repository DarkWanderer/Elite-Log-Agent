using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class MaterialCollected : LogEvent
    {
        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Name_Localised")]
        public string NameLocalised { get; set; }

        [JsonProperty("Count")]
        public long Count { get; set; }
    }
}
