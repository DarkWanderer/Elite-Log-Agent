using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public static class LogEnumerator
    {
        /// <summary>
        /// Returns log file names from freshest to oldest
        /// </summary>
        /// <param name="directory">Path to Elite log directory</param>
        /// <returns></returns>
        public static string[] GetLogFiles(string directory)
        {
            var savedGamesDirectoryInfo = new DirectoryInfo(directory);
            return savedGamesDirectoryInfo
                .GetFiles("*.log", SearchOption.TopDirectoryOnly)
                .OrderByDescending(f => f.Name)
                .Select(f => f.FullName)
                .ToArray();
        }
    }
}
