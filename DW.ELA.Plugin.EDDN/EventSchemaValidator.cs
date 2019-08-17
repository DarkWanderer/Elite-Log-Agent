namespace DW.ELA.Plugin.EDDN
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using DW.ELA.Plugin.EDDN.Model;
    using Newtonsoft.Json.Linq;
    using NJsonSchema;
    using NLog;

    public class EventSchemaValidator
    {
        private IReadOnlyDictionary<string, JsonSchema> schemaCache;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public EventSchemaValidator()
        {
            Log.Debug("Loading EDDN schemas");
            LoadSchemas().GetAwaiter().GetResult();
        }

        public bool ValidateSchema(EddnEvent @event)
        {
            try
            {
                string schemaRef = @event.SchemaRef.Replace("/test", string.Empty);
                if (!schemaCache.ContainsKey(schemaRef))
                {
                    Log.Error("Schema not found: {0}", schemaRef);
                    return false;
                }

                var schema = schemaCache[schemaRef];
                var validationErrors = schema.Validate(JObject.FromObject(@event))
                    .Where(ve => ve.Path != "#/message.prohibited") // Bug in NJsonSchema, TODO
                    .Where(ve => ve.Path != "#/message.economies") // Bug in NJsonSchema, TODO
                    .ToList();
                foreach (var error in validationErrors)
                    Log.Error(error.ToString());

                return validationErrors.Count == 0;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error validating event");
                return false;
            }
        }

        private async Task LoadSchemas()
        {
            var assembly = typeof(EventSchemaValidator).Assembly;
            var resources = assembly.GetManifestResourceNames().Where(r => r.Contains("DW.ELA.Plugin.EDDN.Schemas"));
            if (!resources.Any())
                throw new ApplicationException("Unable to load any schemas");

            var schemas = new Dictionary<string, JsonSchema>();
            foreach (string resource in resources)
            {
                using (var stream = assembly.GetManifestResourceStream(resource))
                using (var reader = new StreamReader(stream))
                {
                    string json = await reader.ReadToEndAsync();
                    var schema = await JsonSchema.FromJsonAsync(json);
                    schemas.Add(schema.Id.TrimEnd('#'), schema);
                    Log.Trace("Loaded schema {0}", schema.Id);
                }
            }
            schemaCache = schemas;
        }
    }
}
