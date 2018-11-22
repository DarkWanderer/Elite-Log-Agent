namespace ELA.Plugin.EDSM
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DW.ELA.Interfaces;
    using DW.ELA.Utility;
    using Newtonsoft.Json.Linq;

    public class EdsmApiFacade : IEdsmApiFacade
    {
        private readonly IRestClient restClient;
        private readonly string commanderName;
        private readonly string apiKey;

        public EdsmApiFacade(IRestClient restClient, string commanderName, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(commanderName))
                throw new ArgumentException(nameof(commanderName));
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException(nameof(apiKey));
            this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            this.commanderName = commanderName;
            this.apiKey = apiKey;
        }

        public async Task PostLogEvents(params JObject[] events)
        {
            if (events.Length == 0)
                return;
            if (events.Length > 100)
                throw new ArgumentException("EDSM cannot accept more than 100 events in single batch");

            var input = CreateHeader();
            input["message"] = new JArray(events).ToString();
            var result = await PostAsync(input);
            var returnCode = JObject.Parse(result)["msgnum"].ToObject<int>();
        }

        private async Task<string> PostAsync(IDictionary<string, string> input)
        {
            var result = await restClient.PostAsync(input);
            var jResult = JObject.Parse(result);
            var returnCode = jResult["msgnum"]?.ToObject<int>();
            var msg = jResult["msg"]?.ToString();
            if (returnCode != 100)
                throw new EdsmApiException(msg);
            return result;
        }

        public async Task<JObject> GetCommanderRanks()
        {
            var input = CreateHeader();
            return JObject.Parse(await PostAsync(input));
        }

        private IDictionary<string, string> CreateHeader()
        {
            return new Dictionary<string, string>
            {
                ["commanderName"] = commanderName,
                ["apiKey"] = apiKey,
                ["fromSoftware"] = AppInfo.Name,
                ["fromSoftwareVersion"] = AppInfo.Version
            };
        }
    }
}
