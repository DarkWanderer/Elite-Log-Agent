using Newtonsoft.Json;

namespace DW.ELA.Plugin.EDDN
{
    public class EddnSettings
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}