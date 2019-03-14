namespace DW.ELA.UnitTests.Utility
{
    using System.IO;
    using DW.ELA.Interfaces;

    internal class TempDirPathManager : IPathManager
    {
        public string SettingsDirectory => Path.GetTempPath();

        public string LogDirectory => Path.GetTempPath();
    }
}
