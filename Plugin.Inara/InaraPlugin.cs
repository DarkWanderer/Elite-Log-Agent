using System;
using InaraUpdater.Model;
using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json.Linq;
using NLog;
using Utility;

namespace InaraUpdater
{
    public class InaraPlugin : IInaraSettingsProvider, IPlugin
    {
        public string SettingsLabel => "INARA settings";
        public string PluginId => "InaraUploader";
        private InaraEventBroker eventBroker;
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private ISettingsProvider settingsProvider;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public static readonly IRestClient RestClient = new ThrottlingRestClient("https://inara.cz/inapi/v1/");

        public InaraPlugin(IPlayerStateHistoryRecorder playerStateRecorder, ISettingsProvider settingsProvider)
        {
            this.playerStateRecorder = playerStateRecorder;
            this.settingsProvider = settingsProvider;
            ReloadSettings();
        }

        public IObserver<JObject> GetLogObserver()
        {
            return eventBroker;
        }

        internal InaraSettings Settings { get; private set; }
        InaraSettings IInaraSettingsProvider.Settings => Settings;
        internal GlobalSettings GlobalSettings => settingsProvider.Settings;

        private void ReloadSettings()
        {
            Settings = settingsProvider.GetPluginSettings(PluginId)?.ToObject<InaraSettings>() ?? new InaraSettings();
            var settings = Settings;
            eventBroker = new InaraEventBroker(new InaraApiFacade(RestClient, settings.ApiKey, GlobalSettings.CommanderName), playerStateRecorder);
        }

        public AbstractSettingsControl GetPluginSettingsControl() => new InaraSettingsControl() { ActualSettings = Settings, Plugin = this };
    }
}
