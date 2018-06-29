using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public partial class Materials : LogEvent
    {
        [JsonProperty("Raw")]
        public Material[] RawMats { get; set; }

        [JsonProperty("Manufactured")]
        public Material[] Manufactured { get; set; }

        [JsonProperty("Encoded")]
        public Material[] Encoded { get; set; }
    }

    public partial class Material
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Count")]
        public long Count { get; set; }
    }
}
