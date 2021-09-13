using System;
using DW.ELA.Interfaces.Settings;
using DW.ELA.Utility.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace DW.ELA.Utility
{
    public class PluginSettingsFacade<T>
        where T : class, new()
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly string pluginId;

        public PluginSettingsFacade(string pluginId)
        {
            this.pluginId = pluginId;
        }

        public T GetPluginSettings(GlobalSettings settings)
        {
            try
            {
                if (settings.PluginSettings.ContainsKey(pluginId))
                    return settings.PluginSettings[pluginId].ToObject<T>();
            }
            catch (Exception e)
            {
                Log.Error(e);

            }
            return new T();
        }

        public void SetPluginSettings(GlobalSettings settings, T value)
        {
            if (!settings.PluginSettings.ContainsKey(pluginId))
                settings.PluginSettings.Add(pluginId, JObject.FromObject(value, Converter.Serializer));
            else
                settings.PluginSettings[pluginId] = JObject.FromObject(value, Converter.Serializer);
        }
    }
}
