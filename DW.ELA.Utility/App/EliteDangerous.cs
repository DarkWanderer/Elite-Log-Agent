using System.Diagnostics;

namespace DW.ELA.Utility.App
{
    public static class EliteDangerous
    {
        public static bool IsRunning => Process.GetProcessesByName("EliteDangerous64").Length > 0;
    }
}
