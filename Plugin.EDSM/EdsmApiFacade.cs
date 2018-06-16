using Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace ELA.Plugin.EDSM
{
    public class EdsmApiFacade : IEdsmApiFacade
    {
        private readonly IRestClient restClient;
        private readonly string commanderName;
        private readonly string apiKey;

        public EdsmApiFacade(IRestClient restClient, string commanderName, string apiKey)
        {
            if (System.String.IsNullOrWhiteSpace(commanderName))
                throw new System.ArgumentException("message", nameof(commanderName));
            if (System.String.IsNullOrWhiteSpace(apiKey))
                throw new System.ArgumentException("message", nameof(apiKey));
            this.restClient = restClient ?? throw new System.ArgumentNullException(nameof(restClient));
            this.commanderName = commanderName;
            this.apiKey = apiKey;
        }

        public async Task PostLogEvents(JObject[] events)
        {
            var input = CreateHeader();
            input["message"] = new JArray(events).ToString();
            var result = await restClient.PostAsync(input);
        }

        public async Task<JObject> GetCommanderRanks()
        {
            var input = CreateHeader();
            return JObject.Parse(await restClient.PostAsync(input));
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
