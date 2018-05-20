using System;
using Interfaces;
using Newtonsoft.Json.Linq;

namespace PowerplayGoogleSheetReporter
{
    public class PowerplayReporterPlugin : IPlugin
    {
        private readonly ISettingsProvider settingsProvider;

        public PowerplayReporterPlugin(ISettingsProvider settingsProvider) => this.settingsProvider = settingsProvider;

        public string SettingsLabel => "Powerplay Sheet Settings";

        public string PluginId => "PowerPlaySheets";

        public IObserver<JObject> GetLogObserver() => new DummyObserver();

        public AbstractSettingsControl GetPluginSettingsControl() => new SettingsControl();
    }
}
