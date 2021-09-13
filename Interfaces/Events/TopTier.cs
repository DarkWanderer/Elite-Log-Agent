using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class TopTier
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Bonus")]
        public string Bonus { get; set; }
    }
}
