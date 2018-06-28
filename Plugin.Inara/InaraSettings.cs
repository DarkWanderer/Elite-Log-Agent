using Newtonsoft.Json;

namespace DW.ELA.Plugin.Inara
{
    public class InaraSettings
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; internal set; } = "INARA API key not set";

        [JsonProperty("verified")]
        public bool Verified { get; internal set; } = false;
    }
}