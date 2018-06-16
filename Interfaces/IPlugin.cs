using DW.ELA.Interfaces.Settings;
using Newtonsoft.Json.Linq;
using System;

namespace Interfaces
{
    public interface IPlugin
    {
        string PluginName { get; }
        string PluginId { get; }

        /// <summary>
        /// Get an observer for the logs event stream
        /// </summary>
        /// <returns>log event observer</returns>
        IObserver<JObject> GetLogObserver();

        /// <summary>
        /// Gets a control which changes plugin settings
        /// Method is provided with a reference to settings item which can be changed
        /// Form MUST NOT change any global state - only the passed GlobalSettings instance
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>Plugin settings control</returns>
        AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings);
    }
}
