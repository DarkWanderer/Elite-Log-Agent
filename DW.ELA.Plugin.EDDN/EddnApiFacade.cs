using Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace ELA.Plugin.EDDN
{
    public class EddnApiFacade : IEddnApiFacade
    {
        private readonly IRestClient restClient;
        private readonly string commanderName;

        public EddnApiFacade(IRestClient restClient, string commanderName)
        {
            if (System.String.IsNullOrWhiteSpace(commanderName))
                throw new System.ArgumentException(nameof(commanderName));
            this.restClient = restClient ?? throw new System.ArgumentNullException(nameof(restClient));
            this.commanderName = commanderName;

        }

        public async Task PostLogEvents(JObject[] events)
        {
            var input = CreateHeader();
            input["message"] = new JArray(events).ToString();
            var result = await restClient.PostAsync(input);
        }

        private IDictionary<string, string> CreateHeader()
        {
            return new Dictionary<string, string>
            {
                ["uploaderID"] = commanderName,
                ["softwareName"] = AppInfo.Name,
                ["softwareVersion"] = AppInfo.Version
            };
        }
    }
}
