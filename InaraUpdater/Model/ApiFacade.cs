using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InaraUpdater.Model
{
    public class ApiFacade
    {
        private readonly IRestClient client;
        private readonly string apiKey;
        private readonly string commanderName;

        public ApiFacade(IRestClient client, string apiKey, string commanderName)
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

        public async Task<ApiEvent> ApiCall(ApiEvent @event) 
        {
            return (await ApiCall(new[] { @event })).Single();
        }

        public async Task<ICollection<ApiEvent>> ApiCall(IList<ApiEvent> events)
        {
            var inputData = new ApiInputOutput()
            {
                Header = new Header(commanderName, apiKey),
                Events = events
            };
            var inputJson = JsonConvert.SerializeObject(inputData, Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                DateFormatString = "u",
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var outputJson = await client.PostAsync(inputJson);
            var outputData = JsonConvert.DeserializeObject<ApiInputOutput>(outputJson);

            // Verify output
            if (outputData.Events != null)
            {
                var exceptions = new List<InaraApiException>();
                for (int i = 0; i < events.Count; i++) {
                    if (outputData.Events[i].EventStatus != 200)
                    {
                        exceptions.Add(new InaraApiException(
                                        outputData.Events[i].EventStatusText ?? "Unknown Error",
                                        events[i].ToString()
                                        ));
                    }
                }

                if (exceptions.Any())
                    throw new AggregateException("Errors returned on some events from API", exceptions);
            }

            if (outputData.Header.EventStatus != 200)
            {
                throw new ApplicationException($"Error from API: {outputData.Header.EventStatusText}");
            }


            return outputData.Events;
        }
    }
}
