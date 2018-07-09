using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace DW.ELA.Utility.Json
{
    public static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "u",
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal },
                new StringEnumConverter()
            },
        };
    }
}
