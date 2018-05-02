using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json.Linq;

namespace EliteLogAgent
{
    public class FileSettingsStorage : ISettingsProvider
    {
        public GlobalSettings Settings
        {
            get
            {
                return new GlobalSettings();
            }
            set { }
        }

        public JObject GetPluginSettings(string plugin)
        {
            JObject result;
            Settings.PluginSettings.TryGetValue(plugin, out result);
            return result;
        }
    }
}