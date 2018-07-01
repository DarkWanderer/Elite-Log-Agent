using Interfaces;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DW.ELA.Plugin.EDDN
{
    public class EventSchemaValidator
    {
        private readonly ConcurrentDictionary<string, Task<JsonSchema4>> schemaCache = new ConcurrentDictionary<string, Task<JsonSchema4>>();
        private readonly IRestClient restClient;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public EventSchemaValidator(IRestClient restClient)
        {
            this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        public bool ValidateSchema(EddnEvent @event) => ValidateSchemaAsync(@event).Result;

        public async Task<bool> ValidateSchemaAsync(EddnEvent @event) // TODO: accept JObject actually
        {
            var schema = await schemaCache.GetOrAdd(@event.SchemaRef, LoadSchemaAsync);
            var validationErrors = schema.Validate(JObject.FromObject(@event));
            foreach (var error in validationErrors)
                logger.Error(error.ToString());

            return validationErrors.Count == 0;
        }

        private async Task<JsonSchema4> LoadSchemaAsync(string schemaUrl)
        {
            var schemaJson = await restClient.GetAsync(schemaUrl);
            return await JsonSchema4.FromJsonAsync(schemaJson);
        }
    }
}
