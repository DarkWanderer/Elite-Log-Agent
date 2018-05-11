using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace Interfaces
{
    public class AbstractSettingsControl : UserControl
    {
        public virtual JObject Settings { get; set; }
    }
}
