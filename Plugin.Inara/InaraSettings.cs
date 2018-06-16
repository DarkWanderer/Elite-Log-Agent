using Newtonsoft.Json;

namespace InaraUpdater
{
    internal class InaraSettings
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; internal set; } = "INARA API key not set";

        [JsonProperty("verified")]
        public bool Verified { get; internal set; } = false;
    }
}