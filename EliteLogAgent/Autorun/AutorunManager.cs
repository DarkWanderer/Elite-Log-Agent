using Microsoft.Win32;
using System.Reflection;

namespace EliteLogAgent.Autorun
{
    public static class AutorunManager
    {
        private const string autorunRegistryKey = @"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string eliteLogAgentKey = @"EliteLogAgent";

        private static string ExecutablePath => (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location;

        public static bool AutorunEnabled
        {
            get
            {
                using (var readHandle = Registry.CurrentUser.OpenSubKey(autorunRegistryKey, false))
                    return readHandle.GetValue(eliteLogAgentKey) as string == ExecutablePath;
            }
            set
            {
                using (var writeHandle = Registry.CurrentUser.OpenSubKey(autorunRegistryKey, true))
                {
                    if (value && !AutorunEnabled)
                        writeHandle.SetValue(eliteLogAgentKey, ExecutablePath);
                    else if (!value && AutorunEnabled)
                        writeHandle.DeleteValue(eliteLogAgentKey);
                }
            }
        }
    }
}
