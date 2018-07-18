using DW.ELA.Interfaces.Settings;
using System.Windows.Forms;

namespace DW.ELA.Interfaces
{
    public class AbstractSettingsControl : UserControl
    {
        public AbstractSettingsControl()
        {
        }

        /// <summary>
        /// Provides reference to temporary instance of Settings existing in settings form
        /// </summary>
        public GlobalSettings GlobalSettings { get; set; }

        public virtual void SaveSettings() { }
    }
}
