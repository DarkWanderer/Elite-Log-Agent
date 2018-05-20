using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace LogModelClassGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //var events = LogEnumerator.GetLogFiles(new SavedGamesDirectoryHelper().Directory)
            //    .Reverse()
            //    .SelectMany(ReadFileEvents)
            //    .ToList()
            //    .GroupBy(e => e["event"])
            //    .Select(DeduceType);
        }

        
        static IEnumerable<JObject> ReadFileEvents(string file)
        {
            using (var textReader = File.OpenText(file))
            using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent = true })
            {
                var serializer = new JsonSerializer();
                while (jsonReader.Read())
                {
                    yield return (JObject)serializer.Deserialize(jsonReader);
                }
            }
        }
    }
}
