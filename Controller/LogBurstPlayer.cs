using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    /// <summary>
    /// This class replays N last log files to observers - to fill up historic data
    /// </summary>
    public class LogBurstPlayer : AbstractObservable<string>
    {
        public void Play()
        {
            var logLines = LatestLogFiles
                .Take(50)
                .Reverse()
                .SelectMany(File.ReadAllLines)
                .ToList();

            logLines.ForEach(OnNext);
            OnCompleted();
        }

        private IEnumerable<string> LatestLogFiles
        {
            get
            {
                var savedGamesDirectoryInfo = new DirectoryInfo(SavedGamesDirectoryHelper.Directory);
                return savedGamesDirectoryInfo.GetFiles()
                    .OrderByDescending(f => f.CreationTimeUtc).Select(f => f.FullName);
            }
        }
    }
}
