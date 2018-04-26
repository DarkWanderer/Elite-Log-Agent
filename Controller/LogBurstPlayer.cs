using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace Controller
{
    /// <summary>
    /// This class replays N last log files to observers - to fill up historic data
    /// </summary>
    public class LogBurstPlayer : AbstractObservable<JObject>
    {
        private readonly string LogDirectory;

        public LogBurstPlayer(string logDirectory)
        {
            LogDirectory = logDirectory;
        }

        public void Play()
        {
            var files = LogEnumerator.GetLogFiles(LogDirectory)
                .Take(100)
                .Reverse()
                .ToList();

            foreach (var file in files)
                using (var textReader = File.OpenText(file))
                using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent = true })
                {
                    JsonSerializer serializer = new JsonSerializer();
                    while (jsonReader.Read())
                    {
                        var @object = (JObject)serializer.Deserialize(jsonReader);
                        OnNext(@object);
                    }
                }
            OnCompleted();
        }
    }
}
