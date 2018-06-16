using System.Diagnostics;
using System.Reflection;

namespace Utility
{
    public static class AppInfo
    {
        public static string Version
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fileVersionInfo.FileVersion;
            }
        }

        public static string Name => "EliteLogAgent";
    }
}
