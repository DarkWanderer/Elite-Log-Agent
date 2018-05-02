using Interfaces.Settings;
using Newtonsoft.Json.Linq;

namespace Interfaces
{
    public interface ISettingsProvider
    {
        GlobalSettings Settings { get; set; }
        JObject GetPluginSettings(string plugin);
    }
}