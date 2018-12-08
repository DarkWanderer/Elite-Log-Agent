namespace DW.ELA.Utility.Log
{
    using System;
    using System.Diagnostics;
    using NLog;
    using NLog.Fluent;

    public class LoggingTimer : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly string context;

        public LoggingTimer(string context)
        {
            this.context = context;
            stopwatch.Start();
        }

        public LogLevel LogLevel { get; set; } = LogLevel.Debug;

        public TimeSpan Elapsed => stopwatch.Elapsed;

        public void Dispose()
        {
            stopwatch.Stop();
            Log.Log(LogLevel)
                .Message("{0}", context)
                .Property("duration", stopwatch.ElapsedMilliseconds)
                .Write();
        }
    }
}
