using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
            Culture = CultureInfo.InvariantCulture,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal },
                new StringEnumConverter()
            },
        };

        public static JsonSerializer Serializer { get; } = JsonSerializer.Create(Settings);
    }
}
