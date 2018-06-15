using Interfaces.Settings;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace Interfaces
{
    public class AbstractSettingsControl : UserControl
    {
        public AbstractSettingsControl()
        {
            MinimumSize = new System.Drawing.Size(200, 150);
            BorderStyle = BorderStyle.FixedSingle;
        }

        /// <summary>
        /// Provides reference to temporary instance of Settings existing in settings form
        /// </summary>
        public GlobalSettings Settings { get; set; }
    }
}
