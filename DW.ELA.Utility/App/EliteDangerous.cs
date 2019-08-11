using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.Utility.App
{
    public static class EliteDangerous
    {
        public static bool IsRunning => Process.GetProcessesByName("EliteDangerous64").Length > 0;
    }
}
