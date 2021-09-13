using System;
using System.Threading;

namespace EliteLogAgent
{
    public static class SingleLaunch
    {
        private static readonly Mutex mutex = new(true, "EliteLogAgent");

        public static bool IsRunning => !mutex.WaitOne(TimeSpan.FromSeconds(3), true);
    }
}
