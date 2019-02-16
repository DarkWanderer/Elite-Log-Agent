namespace DW.ELA.Plugin.Inara.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DW.ELA.Interfaces;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json;
    using NLog;
    using NLog.Fluent;

    public class InaraApiFacade
    {
        private readonly IRestClient client;
        private readonly string apiKey;
        private readonly string commanderName;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly ICollection<string> ignoredErrors = new HashSet<string>
        {
            "Everything was alright, the near-neutral status just wasn't stored.",
            "There is a newer inventory state recorded already.",
            "This ship was not found but was added automatically.",
            "Some errors in the loadout appeared",
            "There is a newer inventory state recorded already.",
            "No commander by in-game name found, the friendship request was not added.",
            "No items provided, the module storage was just erased."
        };

        public InaraApiFacade(IRestClient client, string apiKey, string commanderName)
        {
            this.client = client;
            this.apiKey = apiKey;
            this.commanderName = commanderName;
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
                    var statusCode = outputData.Events[i].EventStatus;
                    if (statusCode != 200)
                    {
                        var statusText = outputData.Events[i].EventStatusText;

                        if (ignoredErrors.Contains(statusText))
                            continue;

                        var ex = new ApplicationException(statusText ?? "Unknown Error");
                        ex.Data.Add("input", inputData.Events[i].ToString());
                        ex.Data.Add("output", outputData.Events[i].ToString());
                        exceptions.Add(ex);

                        if (statusCode < 300)
                            Log.Warn(ex, "Warning returned from Inara API");
                        else
                            Log.Error(ex, "Error returned from Inara API");
                    }
                }
            }

            if (outputData.Header.EventStatus != 200)
                throw new AggregateException($"Error from API: {outputData.Header.EventStatusText}", exceptions.ToArray());

            Log.Info()
                .Message("Uploaded {0} events", events.Length)
                .Property("eventsCount", events.Length)
                .Write();

            return outputData.Events;
        }

        private struct ApiInputOutput
        {
            [JsonProperty("header")]
            public Header Header;

            [JsonProperty("events")]
            public IList<ApiEvent> Events;

            public override string ToString() => Serialize.ToJson(this);
        }
    }
}
