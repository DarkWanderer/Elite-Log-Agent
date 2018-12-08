namespace EliteLogAgent
{
    using System;
    using System.IO;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json;
    using NLog;

    public class FileSettingsStorage : ISettingsProvider
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly object settingsCacheLock = new object();

        private GlobalSettings settingsCache;

        public event EventHandler SettingsChanged;

        public GlobalSettings Settings
        {
            get
            {
                try
                {
                    lock (settingsCacheLock)
                        if (settingsCache != null)
                            return settingsCache;

                    if (File.Exists(SettingsFilePath))
                        return JsonConvert.DeserializeObject<GlobalSettings>(File.ReadAllText(SettingsFilePath));
                }
                catch (Exception e)
                {
                    Log.Warn(e, "Exception while reading settings, using defaults");
                }
                return GlobalSettings.Defaults;
            }

            set
            {
                lock (settingsCacheLock)
                    settingsCache = null;

                using (var fileStream = File.Open(SettingsFilePath, FileMode.Create))
                using (var streamWriter = new StreamWriter(fileStream))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    Converter.Serializer.Serialize(jsonWriter, value);
                }
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public string SettingsFileDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EliteLogAgent");

        private string SettingsFilePath => Path.Combine(SettingsFileDirectory, "Settings.json");
    }
}