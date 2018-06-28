using System;
using System.Threading;

namespace EliteLogAgent
{
    public static class SingleLaunch
    {
        static private Mutex mutex = new Mutex(true, "EliteLogAgent");
        public static bool IsRunning => !mutex.WaitOne(TimeSpan.FromSeconds(3), true);
    }
}
