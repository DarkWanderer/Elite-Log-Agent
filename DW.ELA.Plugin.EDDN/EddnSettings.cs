using Newtonsoft.Json;

namespace ELA.Plugin.EDDN
{
    internal class EddnSettings
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}