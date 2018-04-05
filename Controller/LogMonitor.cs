using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    /// <summary>
    /// This class runs a background thread to monitor and notify consumers (observers) of new log lines
    /// </summary>
    public class LogMonitor : AbstractObservable<string>
    {
    }
}
