using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Interfaces.Settings
{
    public class GlobalSettings
    {
        public GlobalSettings()
        {
            PluginSettings = new Dictionary<string, JObject>();
        }

        [JsonProperty("pluginSettings")]
        public IDictionary<string, JObject> PluginSettings { get; set; }

        [JsonIgnore]
        public string CommanderName => PluginSettings?["General"]?["commanderName"]?.ToString() ?? "Unknown";

        [JsonIgnore]
        public string LogLevel { get; set; }
    }
}
