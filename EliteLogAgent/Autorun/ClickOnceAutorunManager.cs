namespace EliteLogAgent.Autorun
{
    using System;
    using System.IO;
    using System.Reflection;
    using DW.ELA.Interfaces;
    using Microsoft.Win32;

    public class ClickOnceAutorunManager : IAutorunManager
    {
        private const string AutorunRegistryKey = @"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string EliteLogAgentKey = @"EliteLogAgent";


        public bool AutorunEnabled
        {
            get
            {
                using (var readHandle = Registry.CurrentUser.OpenSubKey(AutorunRegistryKey, false))
                    return readHandle.GetValue(EliteLogAgentKey) as string == ExecutablePath;
            }

            set
            {
                using (var writeHandle = Registry.CurrentUser.OpenSubKey(AutorunRegistryKey, true))
                {
                    if (value && !AutorunEnabled)
                        writeHandle.SetValue(EliteLogAgentKey, ExecutablePath);
                    else if (!value && AutorunEnabled)
                        writeHandle.DeleteValue(EliteLogAgentKey);
                }
            }
        }

        protected virtual string ExecutablePath
        {
            get
            {
                var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                return Path.Combine(baseDir, @"Elite Log Agent\Elite Log Agent.appref-ms");
            }
        }
    }
}
