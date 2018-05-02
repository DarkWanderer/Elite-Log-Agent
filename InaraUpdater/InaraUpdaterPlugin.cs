using System;
using Interfaces;
using Newtonsoft.Json.Linq;

namespace InaraUpdater
{
    public class InaraUpdaterPlugin : IPlugin
    {
        public string SettingsLabel => "INARA settings";
        private InaraEventBroker eventBroker;
        private ISettingsProvider settingsProvider;

        public InaraUpdaterPlugin(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public IObserver<JObject> GetLogObserver()
        {
            return eventBroker;
        }

        public AbstractSettingsControl GetPluginSettingsControl()
        {
            return new InaraSettingsControl();
        }
    }
}
