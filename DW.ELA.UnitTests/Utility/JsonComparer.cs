using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests.Utility
{
    public static class JsonComparer
    {
        /// <summary>
        /// Deep compare two NewtonSoft JObjects. If they don't match, returns text diffs
        /// </summary>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <returns>Text string</returns>

        public static IList<string> Compare(string tokenName, JObject source, JObject target)
        {
            var diffs = new List<string>();
            var srcPropertyNames = source.Properties().Select(p => p.Name);
            var tgtPropertyNames = source.Properties().Select(p => p.Name);
            diffs.AddRange(srcPropertyNames.Except(tgtPropertyNames).Select(p => "Missing on left side: " + p));
            diffs.AddRange(tgtPropertyNames.Except(srcPropertyNames).Select(p => "Missing on right side: " + p));

            foreach (var property in srcPropertyNames.Intersect(tgtPropertyNames))
                diffs.AddRange(Compare($"{tokenName}.{property}", source[property], target[property]));

            return diffs;
        }

        public static IList<string> Compare(string tokenName, JToken t1, JToken t2)
        {
            var diffs = new List<string>();
            if (t1?.GetType() != t2?.GetType())
                diffs.Add($"{tokenName}: Different types: {t1?.GetType()} vs {t2?.GetType()}");
            else
            {
                switch (t1)
                {
                    case JArray a: return Compare(tokenName, t1 as JArray, t2 as JArray);
                    case JObject o: return Compare(tokenName, t1 as JObject, t2 as JObject);
                    default:
                        if (t1.ToString() != t2.ToString())
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
                diffs.AddRange(Compare("{tokenName}[{i}]", source[i], target[i]));

            return diffs;
        }
    }
}
