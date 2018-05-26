using Interfaces.Settings;
using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface ISettingsProvider
    {
        GlobalSettings Settings { get; set; }
        JObject GetPluginSettings(string plugin);
        event EventHandler SettingsChanged;
    }
}