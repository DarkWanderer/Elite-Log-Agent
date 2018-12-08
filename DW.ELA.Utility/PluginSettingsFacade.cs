namespace DW.ELA.Utility
{
    using System;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Utility.Extensions;
    using Newtonsoft.Json.Linq;
    using NLog;

    public class PluginSettingsFacade<T>
        where T : class, new()
    {
        private readonly string pluginId;
        private readonly GlobalSettings globalSettings;

        public PluginSettingsFacade(string pluginId, GlobalSettings globalSettings)
        {
            this.pluginId = pluginId;
            this.globalSettings = globalSettings;
        }

        private JObject GetPluginSettings()
        {
            if (globalSettings.PluginSettings.ContainsKey(pluginId))
                return globalSettings.PluginSettings[pluginId];
            else
                return null;
        }

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        JObject PluginSettings => globalSettings.PluginSettings.GetValueOrDefault(pluginId);

        private void SetPluginSettings(JObject value)
        {
            if (!globalSettings.PluginSettings.ContainsKey(pluginId))
                globalSettings.PluginSettings.Add(pluginId, value);
            else
                globalSettings.PluginSettings[pluginId] = value;
        }

        public T Settings
        {
            get
            {
                try
                {
                    return GetPluginSettings()?.ToObject<T>() ?? new T();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return new T();
                }
            }
            set => SetPluginSettings(JObject.FromObject(value));
        }
    }
}
