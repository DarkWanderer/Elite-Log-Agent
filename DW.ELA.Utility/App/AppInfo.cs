namespace DW.ELA.Utility
{
    using System.Diagnostics;
    using System.Reflection;

    public static class AppInfo
    {
        static AppInfo()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly() ?? typeof(AppInfo).Assembly;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fileVersionInfo.FileVersion;
        }

        public static string Version { get; }

        public static string Name => "EliteLogAgent";
    }
}
