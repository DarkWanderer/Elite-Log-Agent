namespace DW.ELA.Controller
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Timers;
    using DW.ELA.Interfaces;
    using NLog;
    using NLog.Fluent;
    using Utility.Observable;

    /// <summary>
    /// This class runs in background to monitor and notify consumers (observers) of new log events
    /// </summary>
    public class JournalMonitor : LogReader, ILogRealTimeDataSource
    {
        private readonly FileSystemWatcher fileWatcher;
        private string currentFile;
        private readonly string logDirectory;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private long filePosition;
        private readonly object @lock = new object();
        private readonly Timer logFlushTimer = new Timer();
        private readonly BasicObservable<LogEvent> basicObservable = new BasicObservable<LogEvent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="JournalMonitor"/> class.
        /// 
        /// </summary>
        /// <param name="logDirectoryProvider">Log directory name provider</param>
        /// <param name="checkInterval">Check interval in milliseconds</param>
        public JournalMonitor(ILogDirectoryNameProvider logDirectoryProvider, int checkInterval = 5000)
        {
            logDirectory = logDirectoryProvider.Directory;
            fileWatcher = new FileSystemWatcher(logDirectory);

            fileWatcher.Changed += FileWatcher_Event;
            fileWatcher.Created += FileWatcher_Event;
            fileWatcher.NotifyFilter = NotifyFilters.CreationTime |
                                       NotifyFilters.FileName |
                                       NotifyFilters.LastWrite |
                                       NotifyFilters.Size;

            logFlushTimer.AutoReset = true;
            logFlushTimer.Interval = checkInterval; // sometimes the filesystem event does not trigger
            logFlushTimer.Elapsed += (o, e) => Task.Factory.StartNew(() => SendEventsFromJournal(false));
            logFlushTimer.Enabled = true;

            currentFile = LogEnumerator.GetLogFiles(logDirectory).First();
            filePosition = new FileInfo(currentFile).Length;
            SendEventsFromJournal(false);
            fileWatcher.EnableRaisingEvents = true;
            logger.Info("Started monitoring");
        }

        private void FileWatcher_Event(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == ".log")
                SendEventsFromJournal(checkOtherFiles: e.FullPath != currentFile);
        }

        private string GetJsonFileFullPath(string fileName) => Path.Combine(logDirectory, Path.ChangeExtension(fileName, ".json"));

        private void SendEventFromFile(string fullPath)
        {
            try
            {
                var @event = ReadFileEvent(fullPath);
                if (@event != null)
                    basicObservable.OnNext(@event);
            }
            catch (Exception e)
            {
                logger.Error()
                    .Message("Error while reading event file")
                    .Exception(e)
                    .Property("event-file", fullPath)
                    .Write();
            }
        }

        private void SendEventsFromJournal(bool checkOtherFiles)
        {
            lock (@lock)
                try
                {
                    // We are not checking file size to make decision about whether
                    // we should read the file. Reason being - the log write operations
                    // are often buffered, so we need to open the file to flush buffers
                    filePosition = ReadJournalFromPosition(currentFile, filePosition);
                    if (checkOtherFiles)
                    {
                        var latestFile = LogEnumerator.GetLogFiles(logDirectory).First();
                        if (latestFile != currentFile)
                        {
                            currentFile = latestFile;
                            filePosition = ReadJournalFromPosition(currentFile, 0);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error()
                        .Message("Error while reading journal file")
                        .Exception(e)
                        .Property("journal-file", currentFile)
                        .Write();
                    filePosition = new FileInfo(currentFile).Length; // Skipping the 'poisoned' data
                }
        }

        private long ReadJournalFromPosition(string file, long filePosition)
        {
            using (var fileReader = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var textReader = new StreamReader(fileReader))
                {
                    fileReader.Position = filePosition;
                    var events = ReadEventsFromStream(textReader);
                    foreach (var @event in events)
                    {
                        // Outfitting, market, etc. events are just indicators that data must be read from json
                        if (@event.Event == "Outfitting" || @event.Event == "Market" || @event.Event == "Shipyard")
                            SendEventFromFile(GetJsonFileFullPath(@event.Event));
                        else
                            basicObservable.OnNext(@event);
                    }
                    return fileReader.Position;
                }
            }
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    fileWatcher.EnableRaisingEvents = false;
                    fileWatcher.Changed -= FileWatcher_Event;
                    fileWatcher.Created -= FileWatcher_Event;
                    fileWatcher.Dispose();
                    logFlushTimer.Dispose();
                    basicObservable.OnCompleted();
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~JsonLogMonitor() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public IDisposable Subscribe(IObserver<LogEvent> observer) => basicObservable.Subscribe(observer);
    }
}
