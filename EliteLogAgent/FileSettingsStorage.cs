using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.IO;

namespace EliteLogAgent
{
    public class FileSettingsStorage : ISettingsProvider
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public GlobalSettings Settings
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<GlobalSettings>(File.ReadAllText(SettingsFilePath));
                }
                catch (Exception e)
                {
                    Log.Warn(e, "Exception while reading settings, using defaults");
                    return new GlobalSettings();
                }
            }
            set
            {
                Directory.CreateDirectory(SettingsFileDirectory);

                using (FileStream fileStream = File.Open(SettingsFilePath, FileMode.Create))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jsonWriter, value);
                }
            }
        }

        public JObject GetPluginSettings(string plugin)
        {
            JObject result;
            Settings.PluginSettings.TryGetValue(plugin, out result);
            return result;
        }

        private string SettingsFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EliteLogAgent");
        private string SettingsFilePath => Path.Combine(SettingsFileDirectory, "Settings.json");
    }
}