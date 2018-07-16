using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using System;

namespace DW.ELA.Interfaces
{
    public interface IPlugin : IObserver<LogEvent>
    {
        string PluginName { get; }
        string PluginId { get; }

        /// <summary>
        /// Get an observer for the logs event stream
        /// </summary>
        /// <returns>log event observer</returns>
        IObserver<LogEvent> GetLogObserver();

        /// <summary>
        /// Gets a control which changes plugin settings
        /// Method is provided with a reference to settings item which can be changed
        /// Form MUST NOT change any global state - only the passed GlobalSettings instance
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>Plugin settings control</returns>
        AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings);

        /// <summary>
        /// Callback to signal settings have changed and it's time to update
        /// </summary>
        void OnSettingsChanged(object sender, EventArgs e);
    }
}
