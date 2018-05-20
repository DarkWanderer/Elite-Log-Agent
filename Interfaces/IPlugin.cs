using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface IPlugin
    {
        string SettingsLabel { get; }
        string PluginId { get; }

        IObserver<JObject> GetLogObserver();
        AbstractSettingsControl GetPluginSettingsControl();
    }
}
