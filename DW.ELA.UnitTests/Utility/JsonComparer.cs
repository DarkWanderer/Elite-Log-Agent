namespace DW.ELA.UnitTests.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json.Linq;

    public static class JsonComparer
    {
        public static IList<string> Compare(string tokenName, JObject source, JObject target)
        {
            var diffs = new List<string>();
            var srcPropertyNames = source.Properties().Select(p => p.Name).ToList();
            var tgtPropertyNames = target.Properties().Select(p => p.Name).ToList();
            diffs.AddRange(srcPropertyNames.Except(tgtPropertyNames).Select(p => tokenName + ": missing on right side: " + p));
            diffs.AddRange(tgtPropertyNames.Except(srcPropertyNames).Select(p => tokenName + ": missing on left side: " + p));

            foreach (var property in srcPropertyNames.Intersect(tgtPropertyNames))
                diffs.AddRange(Compare($"{tokenName}.{property}", source[property], target[property]));

            return diffs;
        }

        public static IList<string> Compare(string tokenName, JToken t1, JToken t2)
        {
            var diffs = new List<string>();
            if (t1?.GetType() != t2?.GetType())
            {
                diffs.Add($"{tokenName}: Different types: {t1?.GetType().ToString() ?? "null"} vs {t2?.GetType().ToString() ?? "null"}");
            }
            else
            {
                switch (t1)
                {
                    case JArray a:
                        return Compare(tokenName, t1 as JArray, t2 as JArray);
                    case JObject o:
                        return Compare(tokenName, t1 as JObject, t2 as JObject);
                    default:
                        if (Serialize.ToJson(t1) != Serialize.ToJson(t2))
                            return new[] { $"{tokenName}: expected {t1}, got {t2}" };
                        break;
                }
            }
            return diffs;
        }

        public static IList<string> Compare(string tokenName, JArray source, JArray target, string arrayName = "")
        {
            var diffs = new List<string>();
            if (source.Count != target.Count)
                diffs.Add($"Array length differs: {source.Count} vs {target.Count}");
            for (int i = 0; i < Math.Min(source.Count, target.Count); i++)
                diffs.AddRange(Compare($"{tokenName}[{i}]", source[i], target[i]));

            return diffs;
        }
    }
}
