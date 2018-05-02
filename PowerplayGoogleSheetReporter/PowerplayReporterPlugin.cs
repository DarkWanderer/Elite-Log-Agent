using System;
using Interfaces;
using Newtonsoft.Json.Linq;

namespace PowerplayGoogleSheetReporter
{
    public class PowerplayReporterPlugin : IPlugin
    {
        private ISettingsProvider settingsProvider;

        public PowerplayReporterPlugin(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public string SettingsLabel => "Powerplay Sheet Settings";

        public IObserver<JObject> GetLogObserver()
        {
            throw new NotImplementedException();
        }

        public AbstractSettingsControl GetPluginSettingsControl()
        {
            return new SettingsControl();
        }
    }
}
