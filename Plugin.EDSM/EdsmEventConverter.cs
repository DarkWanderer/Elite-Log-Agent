namespace DW.ELA.Plugin.EDSM
{
    using System.Collections.Generic;
    using DW.ELA.Interfaces;
    using Newtonsoft.Json.Linq;

    public class EdsmEventConverter : IEventConverter<JObject>
    {
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;

        public EdsmEventConverter(IPlayerStateHistoryRecorder playerStateRecorder)
        {
            this.playerStateRecorder = playerStateRecorder ?? throw new System.ArgumentNullException(nameof(playerStateRecorder));
        }

        public IEnumerable<JObject> Convert(LogEvent sourceEvent)
        {
            var @event = (JObject)sourceEvent.Raw.DeepClone();
            var timestamp = sourceEvent.Timestamp;
            var starSystem = @event["StarSystem"]?.ToObject<string>() ?? playerStateRecorder.GetPlayerSystem(timestamp);

            @event["_systemName"] = starSystem;
            @event["_shipId"] = playerStateRecorder.GetPlayerShipId(timestamp);
            yield return @event;
        }
    }
}
