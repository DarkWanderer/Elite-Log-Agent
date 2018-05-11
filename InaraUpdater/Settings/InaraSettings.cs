using Newtonsoft.Json;

namespace InaraUpdater
{
    internal class InaraSettings
    {
        [JsonProperty("commanderName")]
        public string CommanderName { get; internal set; } = "EliteLogAgentTestUser";
        [JsonProperty("apiKey")]
        public string ApiKey { get; internal set; } = "7nkcf9cb8vkskwwkk8osck0s0g8k8wckoc8cokg";
    }
}