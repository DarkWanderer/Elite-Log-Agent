using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using DW.ELA.LogModel;
using Interfaces;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using Utility;

namespace DW.ELA.Plugin.EDDN
{
    public class EddnPlugin : AbstractPlugin<EddnEvent,EddnSettings>
    {
        private static readonly IRestClient RestClient = new ThrottlingRestClient("https://eddn.edcd.io:4430/upload/");
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider settingsProvider;
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;

        private readonly IEddnApiFacade apiFacade = new EddnApiFacade(RestClient);
        private readonly EddnEventConverter eventConverter;
        private readonly SchemaManager schemaManager = new SchemaManager();

        protected override IEventConverter<EddnEvent> EventConverter => eventConverter;

        public EddnPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder) : base(settingsProvider)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
            eventConverter = new EddnEventConverter(schemaManager) { UploaderID = settingsProvider.Settings.CommanderName };
            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        public override string PluginName => "EDDN Upload";
        public override string PluginId => "EDDN";


        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => null;
        public override async void FlushEvents(EddnEvent[] events) => await apiFacade.PostEventsAsync(events);
        public override void ReloadSettings()
        {
            eventConverter.UploaderID = settingsProvider.Settings.CommanderName;
        }
    }
}
