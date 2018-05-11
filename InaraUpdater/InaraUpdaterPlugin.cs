using System;
using InaraUpdater.Model;
using Interfaces;
using Newtonsoft.Json.Linq;
using NLog;

namespace InaraUpdater
{
    public class InaraUpdaterPlugin : IPlugin
    {
        public string SettingsLabel => "INARA settings";
        public string PluginId => "InaraUploader";
        private InaraEventBroker eventBroker;
        private ISettingsProvider settingsProvider;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly IRestClient restClient = new ThrottlingRestClient("https://inara.cz/inapi/v1/");

        public InaraUpdaterPlugin(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
            ReloadSettings();
        }

        public IObserver<JObject> GetLogObserver()
        {
            return eventBroker;
        }

        private InaraSettings GetSettingsFromProvider()
        {
            try
            {
                return settingsProvider.GetPluginSettings(PluginId)?.ToObject<InaraSettings>()
                    ?? new InaraSettings();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return new InaraSettings();
            }
        }

        private void ReloadSettings()
        {
            var settings = GetSettingsFromProvider();
            eventBroker = new InaraEventBroker(new ApiFacade(restClient, settings.ApiKey, settings.CommanderName));
        }

        public AbstractSettingsControl GetPluginSettingsControl() => new InaraSettingsControl();
    }
}
