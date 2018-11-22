namespace DW.ELA.Utility.Log
{
    using System;
    using System.Diagnostics;
    using NLog;
    using NLog.Fluent;

    public class LoggingTimer : IDisposable
    {
        private Stopwatch stopwatch = new Stopwatch();
        private readonly string context;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public LogLevel LogLevel { get; set; } = LogLevel.Debug;

        public LoggingTimer(string context)
        {
            this.context = context;
            stopwatch.Start();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            logger.Log(LogLevel)
                .Message("{0}", context)
                .Property("duration", stopwatch.ElapsedMilliseconds)
                .Write();
        }

        public TimeSpan Elapsed => stopwatch.Elapsed;
    }
}
