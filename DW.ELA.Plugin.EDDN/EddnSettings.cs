namespace DW.ELA.Plugin.EDDN
{
    using Newtonsoft.Json;

    public class EddnSettings
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}