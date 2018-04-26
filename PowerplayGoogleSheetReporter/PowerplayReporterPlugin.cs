using Interfaces;

namespace PowerplayGoogleSheetReporter
{
    public class PowerplayReporterPlugin : IPlugin
    {
        public string SettingsLabel => "Powerplay Sheet Settings";

        public AbstractSettingsControl GetPluginSettingsControl()
        {
            return new SettingsControl();
        }
    }
}
