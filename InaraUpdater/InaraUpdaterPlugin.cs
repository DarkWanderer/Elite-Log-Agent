using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InaraUpdater
{
    public class InaraUpdaterPlugin : IPlugin
    {
        public string SettingsLabel => "INARA settings";

        public Control GetPluginSettingsControl()
        {
            return new InaraSettingsControl();
        }
    }
}
