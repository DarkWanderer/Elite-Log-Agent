using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
