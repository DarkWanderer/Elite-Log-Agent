using System;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using InaraUpdater.Model;
using Interfaces;
using Newtonsoft.Json.Linq;
using NLog;
using Utility;

namespace InaraUpdater
{
    public class InaraPlugin : IPlugin
    {
        public string PluginName => CPluginName;
        public string PluginId => CPluginId;

        public const string CPluginName = "INARA settings";
        public const string CPluginId = "InaraUploader";
        public static readonly IRestClient RestClient = new ThrottlingRestClient("https://inara.cz/inapi/v1/");

        private InaraEventBroker eventBroker;
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private ISettingsProvider settingsProvider;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

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

        internal InaraSettings Settings
        {
            get => new PluginSettingsFacade<InaraSettings>(PluginId, GlobalSettings).Settings;
            set => new PluginSettingsFacade<InaraSettings>(PluginId, GlobalSettings).Settings = value;
        }

        private void ReloadSettings()
        {
            eventBroker?.FlushQueue();
            eventBroker = new InaraEventBroker(new InaraApiFacade(RestClient, Settings.ApiKey, GlobalSettings.CommanderName), playerStateRecorder);
        }

        public AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new InaraSettingsControl() { GlobalSettings = settings };
        public void OnSettingsChanged(object o,EventArgs e) => ReloadSettings();
    }
}
