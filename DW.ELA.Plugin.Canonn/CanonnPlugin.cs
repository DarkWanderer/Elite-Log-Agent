using DW.ELA.Controller;
using DW.ELA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.Plugin.Canonn
{
    public class CanonnPlugin : AbstractPlugin<object, CanonnApiSettings>
    {
        protected CanonnPlugin(ISettingsProvider settingsProvider) : base(settingsProvider)
        {
        }

        public override string PluginName => "Canonn";
        public override string PluginId => "Canonn";
        protected override IEventConverter<object> EventConverter { get; }

        public override void FlushEvents(ICollection<object> events) => throw new NotImplementedException();
        public override AbstractSettingsControl GetPluginSettingsControl(ELA.Interfaces.Settings.GlobalSettings settings) => throw new NotImplementedException();
        public override void ReloadSettings() => throw new NotImplementedException();
    }
}
