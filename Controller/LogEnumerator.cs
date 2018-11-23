namespace DW.ELA.Controller
{
    using System.IO;
    using System.Linq;

    public static class LogEnumerator
    {
        /// <summary>
        /// Returns log file names from freshest to oldest
        /// </summary>
        /// <param name="directory">Path to Elite log directory</param>
        /// <returns>array of file names</returns>
        public static string[] GetLogFiles(string directory)
        {
            var savedGamesDirectoryInfo = new DirectoryInfo(directory);
            return savedGamesDirectoryInfo
                .GetFiles("Journal.*.log", SearchOption.TopDirectoryOnly)
                .OrderByDescending(f => f.Name)
                .Select(f => f.FullName)
                .ToArray();
        }

        /// <summary>
        /// Returns list of static event files (Outfitting.json, Market.json, etc.)
        /// </summary>
        /// <param name="directory">Path to Elite log directory</param>
        /// <returns>array of file names</returns>
        public static string[] GetJsonEventFiles(string directory)
        {
            var savedGamesDirectoryInfo = new DirectoryInfo(directory);
            return savedGamesDirectoryInfo
                .GetFiles("*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(f => f.Name)
                .Select(f => f.FullName)
                .ToArray();
        }
    }
}
