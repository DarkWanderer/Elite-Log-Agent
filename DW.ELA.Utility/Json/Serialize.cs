using System;
using Newtonsoft.Json;

namespace DW.ELA.Utility.Json
{
    public static class Serialize
    {
        public static string ToJson<T>(this T self) => JsonConvert.SerializeObject(self, Converter.Settings);

        public static T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json, Converter.Settings);

        public static object FromJson(Type type, string json) => JsonConvert.DeserializeObject(json, type, Converter.Settings);
    }
}
