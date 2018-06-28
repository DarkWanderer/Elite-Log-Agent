using Interfaces;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;
using Newtonsoft.Json.Schema;
using System.Reflection;
using System.IO;

namespace DW.ELA.Plugin.EDDN
{
    public class EddnApiFacade : IEddnApiFacade
    {
        private readonly IRestClient restClient;

        public EddnApiFacade(IRestClient restClient)
        {
            this.restClient = restClient ?? throw new System.ArgumentNullException(nameof(restClient));
        }

        //private JSchema GetSchema(EddnSchemaType type)
        //{
        //    var resourceName = "DW.ELA.Plugin.EDDN.Schemas." + type.ToString().ToLowerInvariant() + ".json";
        //    var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        //    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        //    using (var reader = new StreamReader(stream))
        //    {
        //        string result = reader.ReadToEnd();
        //        return JSchema.Parse(result);
        //    }
        //}

        public async Task PostMessage(EddnSchemaType schemaType, JObject message)
        {
            var input = CreateHeader();
            input["message"] = message.ToString();
            var result = await restClient.PostAsync(input);
        }

        private IDictionary<string, string> CreateHeader()
        {
            return new Dictionary<string, string>
            {
                //["uploaderID"] = commanderName,
                ["softwareName"] = AppInfo.Name,
                ["softwareVersion"] = AppInfo.Version
            };
        }
    }
}
