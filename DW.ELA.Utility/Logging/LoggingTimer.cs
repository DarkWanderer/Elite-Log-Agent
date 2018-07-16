using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.Utility.Log
{
    public class LoggingTimer : IDisposable
    {
        private Stopwatch stopwatch = new Stopwatch();
        private readonly string context;

        public LoggingTimer(string context)
        {
            this.context = context;
            stopwatch.Start();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            LogManager.GetCurrentClassLogger().Debug("{0} took {1}ms", context, stopwatch.ElapsedMilliseconds);
        }

        public TimeSpan Elapsed => stopwatch.Elapsed;
    }
}
