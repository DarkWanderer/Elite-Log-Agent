using System;
using InaraUpdater.Model;
using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json.Linq;
using NLog;
using Utility;

namespace InaraUpdater
{
    public class InaraPlugin : IPlugin
    {
        public string PluginName => "INARA settings";
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

        internal GlobalSettings GlobalSettings => settingsProvider.Settings;

        private void ReloadSettings()
        {
            //eventBroker = new InaraEventBroker(new InaraApiFacade(restClient, settings.ApiKey, GlobalSettings.CommanderName), playerStateRecorder);
        }

        public AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new InaraSettingsControl() { Settings = settings, Plugin = this };
    }
}
