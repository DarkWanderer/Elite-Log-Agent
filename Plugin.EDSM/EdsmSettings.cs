namespace DW.ELA.Plugin.EDSM
{
    using Newtonsoft.Json;

    public class EdsmSettings
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; internal set; } = "EDSM API Key not set";

        [JsonProperty("verified")]
        public bool Verified { get; internal set; } = false;
    }
}
