using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DW.ELA.Utility
{
    public static class AppInfo
    {
        static AppInfo()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly() ?? typeof(AppInfo).Assembly;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fileVersionInfo.FileVersion;
            
            var clickOnceInstallationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Apps");
            IsNetworkDeployed = assembly.Location.StartsWith(clickOnceInstallationDirectory);
        }

        public static string Version { get; }

        public static string Name => "EliteLogAgent";

        public static bool IsNetworkDeployed { get; }
    }
}
