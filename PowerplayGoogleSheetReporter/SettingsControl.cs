using Interfaces;
using Newtonsoft.Json.Linq;

namespace PowerplayGoogleSheetReporter
{
    public class SettingsControl : AbstractSettingsControl
    {
        public override JObject Settings { get => new JObject(); }
    }
}