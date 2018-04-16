using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interfaces
{
    public interface IPlugin
    {
        string SettingsLabel { get; }

        //IObserver<JObject> GetLogObserver();
        Control GetPluginSettingsControl();
    }
}
