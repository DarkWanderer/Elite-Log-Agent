using DW.ELA.Utility;
using DW.ELA.Utility.Json;
using Interfaces;
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

            public override string ToString() => JsonConvert.SerializeObject(this);
        }

        public async Task<ApiEvent> ApiCall(ApiEvent @event) => (await ApiCall(new[] { @event })).Single();

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

            // Verify output
            if (outputData.Events != null)
            {
                var exceptions = new List<InaraApiException>();
                for (int i = 0; i < events.Length; i++) {
                    if (outputData.Events[i].EventStatus != 200)
                    {
                        exceptions.Add(new InaraApiException(
                                        outputData.Events[i].EventStatusText ?? "Unknown Error",
                                        events[i].ToString()
                                        ));
                    }
                }
            }

            if (outputData.Header.EventStatus != 200)
            {
                throw new ApplicationException($"Error from API: {outputData.Header.EventStatusText}");
            }


            return outputData.Events;
        }
    }
}
