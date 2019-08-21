namespace DW.ELA.Plugin.Inara.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly string frontierID;
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

        public InaraApiFacade(IRestClient client, string commanderName, string apiKey, string frontierID = null)
        {
            this.client = client;
            this.apiKey = apiKey;
            this.commanderName = commanderName;
            this.frontierID = frontierID;
        }

        public async Task<ICollection<ApiOutputEvent>> ApiCall(params ApiInputEvent[] events)
        {
            if (events.Length == 0)
                return new ApiOutputEvent[0];

            var inputData = new ApiInputBatch()
            {
                Header = new Header(commanderName, apiKey, frontierID),
                Events = events
            };
            string inputJson = inputData.ToJson();
            string outputJson = await client.PostAsync(inputJson);
            var outputData = JsonConvert.DeserializeObject<ApiOutputBatch>(outputJson);

            var exceptions = new List<Exception>();

            // Verify output
            if (outputData.Events != null)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    var outputEvent = outputData.Events[i];
                    int? statusCode = outputEvent.EventStatus;
                    string statusText = outputEvent.EventStatusText;

                    if (statusCode != 200)
                    {
                        if (ignoredErrors.Contains(statusText))
                            continue;

                        exceptions.Add(new ApplicationException(statusText ?? "Unknown Error"));

                        var log = statusCode < 300 ? Log.Warn() : Log.Error();
                        log.Message(statusText)
                            .Property("input", inputData.Events[i].ToString())
                            .Property("output", outputEvent.ToString())
                            .Property("status", statusCode)
                            .Write();
                    }
                }
            }

            if (outputData.Header.EventStatus != 200)
                throw new AggregateException($"Error from API: {outputData.Header.EventStatusText}", exceptions.ToArray());

            return outputData.Events;
        }

        public async Task<string> GetCmdrName()
        {
            var @event = new ApiInputEvent("getCommanderProfile") { EventData = new Dictionary<string, object>(), Timestamp = DateTime.Now };
            var result = (await ApiCall(@event)).SingleOrDefault();
            if (result == null)
                throw new ApplicationException("Null result from API");
            string cmdrName = (result.EventData as dynamic).commanderName ?? "Error: cmdr name was not returned";
            return cmdrName;
        }

    }
}
