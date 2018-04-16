using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerplayGoogleSheetReporter
{
    public class PowerplayReporterPlugin : IPlugin
    {
        public string SettingsLabel => "Powerplay Sheet Settings";

        public Control GetPluginSettingsControl()
        {
            return new Label() { Text = SettingsLabel };
        }
    }
}
