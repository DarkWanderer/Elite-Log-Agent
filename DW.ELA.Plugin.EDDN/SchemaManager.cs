using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Utility;

namespace DW.ELA.Plugin.EDDN
{
    public class SchemaManager
    {
        private static int GetSchemaVersion(EddnSchemaType schemaType)
        {
            switch (schemaType)
            {
                case EddnSchemaType.Blackmarket: return 1;
                case EddnSchemaType.Commodity: return 3;
                case EddnSchemaType.Journal: return 1;
                case EddnSchemaType.Outfitting: return 2;
                case EddnSchemaType.Shipyard: return 2;
                default: throw new ArgumentOutOfRangeException(nameof(schemaType));
            }
        }

        public static string GetSchemaUrl(EddnSchemaType schemaType) => $"https://eddn.edcd.io/schemas/{schemaType.ToString().ToLower()}/{GetSchemaVersion(schemaType).ToString(CultureInfo.InvariantCulture)}";

        private readonly Task<IReadOnlyDictionary<EddnSchemaType, string>> schemas;
        private static readonly WebClient webClient = new WebClient();

        public SchemaManager()
        {
            //schemas = Task.Factory.StartNew(LoadSchemas);
        }

        //private IReadOnlyDictionary<EddnSchemaType, string> LoadSchemas()
        //{
        //    // Using System.Net.WebClient to download as it provides caching functionality
        //    //var schemaTasks = Enum.GetValues(typeof(EddnSchemaType))
        //    //    .Cast<EddnSchemaType>()
        //    //    .ToDictionary(st => st, st => webClient.DownloadStringTaskAsync(new Uri(GetSchemaUrl(st))));
        //    //Task.WaitAll(schemaTasks.Values.ToArray());
        //    //return schemaTasks.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Result);
        //}

        public async Task<bool> Validate(EddnSchemaType schemaType, string json)
        {
            var schema = await JsonSchema4.FromJsonAsync(schemas.Result[schemaType]);
            return !schema.Validate(json).Any();
        }
    }
}
