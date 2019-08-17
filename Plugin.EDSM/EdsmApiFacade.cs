namespace DW.ELA.Plugin.EDSM
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
            string result = await PostAsync(input);
            int returnCode = JObject.Parse(result)["msgnum"].ToObject<int>();
        }

        private async Task<string> PostAsync(IDictionary<string, string> input)
        {
            string result = await restClient.PostAsync(input);
            var jResult = JObject.Parse(result);
            int? returnCode = jResult["msgnum"]?.ToObject<int>();
            string msg = jResult["msg"]?.ToString();
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
