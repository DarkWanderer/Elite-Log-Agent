using System;
using InaraUpdater.Model;
using Interfaces;
using Newtonsoft.Json.Linq;
using NLog;

namespace InaraUpdater
{
    public class InaraUpdaterPlugin : IInaraSettingsProvider, IPlugin
    {
        public string SettingsLabel => "INARA settings";
        public string PluginId => "InaraUploader";
        private InaraEventBroker eventBroker;
        private ISettingsProvider settingsProvider;
        private readonly ILogger Log;
        private static readonly IRestClient restClient = new ThrottlingRestClient("https://inara.cz/inapi/v1/");

        public InaraUpdaterPlugin(ISettingsProvider settingsProvider, ILogger logger)
        {
            this.settingsProvider = settingsProvider;
            Log = logger;
            ReloadSettings();
        }

        public IObserver<JObject> GetLogObserver()
        {
            return eventBroker;
        }

        internal InaraSettings Settings
        {
            get
            {
                try
                {
                    return settingsProvider.GetPluginSettings(SettingsLabel)?.ToObject<InaraSettings>()
                        ?? new InaraSettings();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return new InaraSettings();
                }
            }
        }

        InaraSettings IInaraSettingsProvider.Settings => Settings;

        private void ReloadSettings()
        {
            var settings = Settings;
            eventBroker = new InaraEventBroker(new ApiFacade(restClient, settings.ApiKey, settings.CommanderName), Log);
        }

        public AbstractSettingsControl GetPluginSettingsControl() => new InaraSettingsControl() { ActualSettings = Settings };
    }
}
