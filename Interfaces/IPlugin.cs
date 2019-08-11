namespace DW.ELA.Interfaces
{
    using System;
    using DW.ELA.Interfaces.Settings;

    public interface IPlugin
    {
        string PluginName { get; }

        string PluginId { get; }

        /// <summary>
        /// Get an observer for the logs event stream
        /// </summary>
        /// <returns>log event observer</returns>
        IObserver<JournalEvent> GetLogObserver();

        /// <summary>
        /// Gets a control which changes plugin settings
        /// Method is provided with a reference to settings item which can be changed
        /// Form MUST NOT change any global state - only the passed GlobalSettings instance
        /// </summary>
        /// <param name="settings">Instance of temporary settings object held in setup session</param>
        /// <returns>Plugin settings control</returns>
        AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings);

        /// <summary>
        /// Callback to signal settings have changed and it's time to update
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event data</param>
        void OnSettingsChanged(object sender, EventArgs e);

        /// <summary>
        /// Explicitly request to flush queue - on shutdown
        /// </summary>
        [Obsolete]
        void FlushQueue();
    }
}
