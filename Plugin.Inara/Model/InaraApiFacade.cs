using DW.ELA.Utility;
using DW.ELA.Utility.Json;
using DW.ELA.Interfaces;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DW.ELA.Plugin.Inara.Model
{
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

            var exceptions = new List<InaraApiException>();
            // Verify output
            if (outputData.Events != null)
            {
                for (int i = 0; i < events.Length; i++) {
                    if (outputData.Events[i].EventStatus != 200)
                    {
                        var ex = new InaraApiException(
                                        outputData.Events[i].EventStatusText ?? "Unknown Error",
                                        events[i].ToString());
                        exceptions.Add(ex);
                        logger.Error(ex, "Error returned from Inara API");
                    }
                }
            }

            if (outputData.Header.EventStatus != 200)
                throw new AggregateException($"Error from API: {outputData.Header.EventStatusText}", exceptions.ToArray());

            return outputData.Events;
        }
    }
}
