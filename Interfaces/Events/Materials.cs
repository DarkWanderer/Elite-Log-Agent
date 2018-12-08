namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public partial class Materials : LogEvent
    {
        [JsonProperty("Raw")]
        public Material[] RawMats { get; set; }

        [JsonProperty("Manufactured")]
        public Material[] Manufactured { get; set; }

        [JsonProperty("Encoded")]
        public Material[] Encoded { get; set; }
    }
}
