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
            //Size = new System.Drawing.Size(200, 150);
            //AutoSizeMode = AutoSizeMode.GrowOnly;
        }

        public virtual JObject Settings { get; set; }
    }
}
