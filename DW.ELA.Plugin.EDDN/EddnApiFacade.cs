namespace DW.ELA.Plugin.EDDN
{
    using System;
    using System.Threading.Tasks;
    using DW.ELA.Interfaces;
    using DW.ELA.Plugin.EDDN.Model;
    using DW.ELA.Utility.Json;
    using NLog;
    using NLog.Fluent;

    public class EddnApiFacade : IEddnApiFacade
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly IRestClient restClient;

        public EddnApiFacade(IRestClient restClient)
        {
            this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        public async Task PostEventAsync(EddnEvent @event)
        {
            var eventData = Serialize.ToJson(@event);
            try
            {
                var result = await restClient.PostAsync(eventData);
                if (result != "OK")
                    Log.Error("Error pushing event: {0}", result);
                else
                    Log.Info("Pushed event {0}", @event.GetType());
            }
            catch (Exception e)
            {
                Log.Error()
                    .Message("Error pushing event")
                    .Exception(e)
                    .Property("input", eventData)
                    .Write();
            }
        }
    }
}
