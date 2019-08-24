namespace EliteLogAgent
{
    using System;
    using System.Threading;

    public static class SingleLaunch
    {
        private static readonly Mutex mutex = new Mutex(true, "EliteLogAgent");

        public static bool IsRunning => !mutex.WaitOne(TimeSpan.FromSeconds(3), true);
    }
}
