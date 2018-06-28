using DW.ELA.Controller;
using DW.ELA.Interfaces.Settings;
using DW.ELA.LogModel;
using Interfaces;
using NLog;
using System;
using Utility;

namespace DW.ELA.Plugin.EDDN
{
    public class EddnPlugin : AbstractPlugin<EddnSettings>
    {
        private static readonly IRestClient RestClient = new ThrottlingRestClient("https://eddn.edcd.io:4430/upload/");
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private readonly IEddnApiFacade apiFacade = new EddnApiFacade(RestClient);

        public EddnPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder) : base(settingsProvider)
        {
            if (settingsProvider == null)
                throw new ArgumentNullException(nameof(settingsProvider));
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        public override string PluginName => "EDDN Upload";
        public override string PluginId => "EDDN";

        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => null;
        public override void ProcessEvents(LogEvent[] events) {}
        public override void ReloadSettings() { }
    }
}
