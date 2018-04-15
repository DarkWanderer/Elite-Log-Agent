using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Controller
{
    /// <summary>
    /// This class replays N last log files to observers - to fill up historic data
    /// </summary>
    public class LogBurstPlayer : AbstractObservable<JObject>
    {
        public void Play()
        {
            var files = LatestLogFiles.Reverse().ToList();

            foreach (var file in files)
                using (var textReader = File.OpenText(file))
                using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent  = true})
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

        private IEnumerable<string> LatestLogFiles
        {
            get
            {
                //var savedGamesDirectoryInfo = new DirectoryInfo(SavedGamesDirectoryHelper.Directory);
                var savedGamesDirectoryInfo = new DirectoryInfo(@"D:\Oleg\Projects\Elite-Log-Agent\LogSamples");
                return savedGamesDirectoryInfo.GetFiles()
                    .OrderByDescending(f => f.Name).Select(f => f.FullName);
            }
        }
    }
}
