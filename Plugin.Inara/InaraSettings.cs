namespace DW.ELA.Plugin.Inara
{
    using Newtonsoft.Json;

    public class InaraSettings
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; internal set; } = "INARA API key not set";

        [JsonProperty("verified")]
        public bool Verified { get; internal set; } = false;
    }
}