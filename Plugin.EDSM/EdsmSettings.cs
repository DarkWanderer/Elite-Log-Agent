using Newtonsoft.Json;

namespace ELA.Plugin.EDSM
{
    public class EdsmSettings
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; internal set; } = "EDSM API Key not set";

        [JsonProperty("verified")]
        public bool Verified { get; internal set; } = false;
    }
}
