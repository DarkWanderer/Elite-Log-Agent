namespace DW.ELA.Plugin.EDDN
{
    using System;
    using System.Threading.Tasks;
    using DW.ELA.Interfaces;
    using DW.ELA.Plugin.EDDN.Model;
    using DW.ELA.Utility.Json;
    using NLog;

    public class EddnApiFacade : IEddnApiFacade
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IRestClient restClient;

        public EddnApiFacade(IRestClient restClient)
        {
            this.restClient = restClient ?? throw new System.ArgumentNullException(nameof(restClient));
        }

        public async Task PostEventsAsync(params EddnEvent[] events)
        {
            foreach (var @event in events)
            {
                try
                {
                    await PostAsync(@event);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error pushing event");
                }
            }

            logger.Info("Pushed {0} events", events.Length);
        }

        private async Task PostAsync(EddnEvent e)
        {
            var result = await restClient.PostAsync(Serialize.ToJson(e));
            if (result != "OK")
                logger.Error(result);
        }
    }
}
