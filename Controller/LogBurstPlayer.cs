using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using Utility.Observable;

namespace Controller
{
    /// <summary>
    /// This class replays N last log files to observers - to fill up historic data
    /// </summary>
    public class LogBurstPlayer : AbstractObservable<JObject>
    {
        private readonly string LogDirectory;
        private readonly int filesNumber;

        public LogBurstPlayer(string logDirectory, int filesNumber = 5)
        {
            if (string.IsNullOrEmpty(logDirectory))
                throw new System.ArgumentException("Must provide log directory", nameof(logDirectory));
            if (filesNumber <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(logDirectory), filesNumber, "nubmer of files must be > 0");

            LogDirectory = logDirectory;
            this.filesNumber = filesNumber;
        }

        public void Play()
        {
            var files = LogEnumerator.GetLogFiles(LogDirectory)
                .Take(filesNumber)
                .Reverse()
                .ToList();

            foreach (var file in files)
                using (var fileReader = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var textReader = new StreamReader(fileReader))
                using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent = true })
                {
                    var serializer = new JsonSerializer();
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
