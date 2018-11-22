namespace DW.ELA.Plugin.Inara.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DW.ELA.Interfaces;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json;
    using NLog;

    public class InaraApiFacade
    {
        private readonly IRestClient client;
        private readonly string apiKey;
        private readonly string commanderName;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public InaraApiFacade(IRestClient client, string apiKey, string commanderName)
        {
            this.client = client;
            this.apiKey = apiKey;
            this.commanderName = commanderName;
        }

        private struct ApiInputOutput
        {
            [JsonProperty("header")]
            public Header Header;
            [JsonProperty("events")]
            public IList<ApiEvent> Events;

            public override string ToString() => Serialize.ToJson(this);
        }

        public async Task<ICollection<ApiEvent>> ApiCall(params ApiEvent[] events)
        {
            if (events.Length == 0)
                return new ApiEvent[0];

            var inputData = new ApiInputOutput()
            {
                Header = new Header(commanderName, apiKey),
                Events = events
            };
            var inputJson = inputData.ToJson();
            var outputJson = await client.PostAsync(inputJson);
            var outputData = JsonConvert.DeserializeObject<ApiInputOutput>(outputJson);

            var exceptions = new List<Exception>();

            // Verify output
            if (outputData.Events != null)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    if (outputData.Events[i].EventStatus != 200)
                    {
                        var statusText = outputData.Events[i].EventStatusText;
                        if (statusText.StartsWith("Some errors in the loadout appeared"))
                            continue; // Skip the errors related to data missing on Inara side
                        if (statusText == "There is a newer inventory state recorded already.")
                            continue; // Not really an error

                        var ex = new ApplicationException(statusText ?? "Unknown Error");
                        ex.Data.Add("input", inputData.Events[i].ToString());
                        ex.Data.Add("output", outputData.Events[i].ToString());
                        exceptions.Add(ex);
                        logger.Error(ex, "Error returned from Inara API");
                    }
                }
            }

            if (outputData.Header.EventStatus != 200)
                throw new AggregateException($"Error from API: {outputData.Header.EventStatusText}", exceptions.ToArray());
            logger.Info("Uploaded {0} INARA events", events.Length);
            return outputData.Events;
        }
    }
}
