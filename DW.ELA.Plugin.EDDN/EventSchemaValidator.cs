using Interfaces;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DW.ELA.Plugin.EDDN
{
    public class EventSchemaValidator
    {
        private IReadOnlyDictionary<string, JsonSchema4> schemaCache;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public EventSchemaValidator()
        {
            LoadSchemas().Wait();
        }

        private async Task LoadSchemas()
        {
            var assembly = typeof(EventSchemaValidator).Assembly;
            var resources = assembly.GetManifestResourceNames().Where(r => r.Contains("DW.ELA.Plugin.EDDN.Schemas"));
            if (!resources.Any())
                throw new ApplicationException("Unable to load any schemas");

            var schemas = new Dictionary<string, JsonSchema4>();
            foreach (var resource in resources)
            {
                using (var stream = assembly.GetManifestResourceStream(resource))
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    var schema = await JsonSchema4.FromJsonAsync(json);
                    schemas.Add(schema.Id.TrimEnd('#'), schema);
                }
            }
            schemaCache = schemas;
        }

        public bool ValidateSchema(EddnEvent @event)
        {
            try
            {
                if (!schemaCache.ContainsKey(@event.SchemaRef))
                    return false;

                var schema = schemaCache[@event.SchemaRef];
                var validationErrors = schema.Validate(JObject.FromObject(@event));
                foreach (var error in validationErrors)
                    logger.Error(error.ToString());

                return validationErrors.Count == 0;
            }
            catch (Exception e)
            {
                logger.Error(e, "Error validating event");
                return false;
            }
        }
    }
}
