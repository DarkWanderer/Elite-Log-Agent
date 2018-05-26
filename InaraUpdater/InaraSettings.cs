using Newtonsoft.Json;

namespace InaraUpdater
{
    internal class InaraSettings
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; internal set; } = "Not set";

        [JsonProperty("verified")]
        public bool Verified { get; internal set; } = false;
    }
}