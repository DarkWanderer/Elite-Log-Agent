using System.IO;
using DW.ELA.Interfaces;

namespace DW.ELA.UnitTests.Utility
{
    internal class TempDirPathManager : IPathManager
    {
        public string SettingsDirectory => Path.GetTempPath();

        public string LogDirectory => Path.GetTempPath();
    }
}
