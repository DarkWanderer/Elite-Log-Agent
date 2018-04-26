using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace Interfaces
{
    abstract public class AbstractSettingsControl : UserControl
    {
        public abstract JObject Settings { get; set; }
    }
}
