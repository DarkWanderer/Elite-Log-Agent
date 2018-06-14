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

        [JsonProperty("commanderName")]
        public string CommanderName { get; set; } = "Unknown Commander";

        [JsonProperty("logLevel")]
        public string LogLevel { get; set; } = "Info";

        [JsonProperty("setupWizardDone")]
        public bool InitialSetupDone { get; set; } = false;
    }
}
