using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.Utility.Json
{
    public static class JObjectExtensions
    {
        public static void AddIfNotNull<TValue>(this JObject jObject, string key, TValue value)
        {
            if (value != null)
                jObject.Add(key, JToken.FromObject(value));
        }
    }
}
