using System;
using System.IO;
using DW.ELA.Interfaces;
using NLog;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Utility.Observable;

namespace Controller
{
    /// <summary>
    /// This class runs in background to monitor and notify consumers (observers) of new log events
    /// </summary>
    public class JournalMonitor : LogReader, ILogRealTimeDataSource
    {
        private readonly FileSystemWatcher fileWatcher;
        private string CurrentFile;
        private readonly string LogDirectory;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private long filePosition;
        private readonly object @lock = new object();
        private readonly Timer logFlushTimer = new Timer();
        private readonly BasicObservable<LogEvent> basicObservable = new BasicObservable<LogEvent>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logDirectoryProvider">Log directory name provider</param>
        /// <param name="checkInterval">Check interval in milliseconds</param>
        public JournalMonitor(ILogDirectoryNameProvider logDirectoryProvider, int checkInterval = 5000)
        {
            LogDirectory = logDirectoryProvider.Directory;
            fileWatcher = new FileSystemWatcher(LogDirectory);

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

            CurrentFile = LogEnumerator.GetLogFiles(LogDirectory).First();
            filePosition = new FileInfo(CurrentFile).Length;
            SendEventsFromJournal(false);
            fileWatcher.EnableRaisingEvents = true;
            logger.Info("Started monitoring on folder {0}", LogDirectory);
        }

        private void FileWatcher_Event(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == ".log")
                SendEventsFromJournal(checkOtherFiles: e.FullPath != CurrentFile);
            else if (Path.GetFileName(e.FullPath) == "Outfitting.json" || Path.GetFileName(e.FullPath) == "Market.json")
                SendEventFromFile(e.FullPath);
        }

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
                logger.Error(e, "Error while reading event file {0}", fullPath);
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
                    filePosition = ReadFileFromPosition(CurrentFile, filePosition);
                    if (checkOtherFiles)
                    {
                        var latestFile = LogEnumerator.GetLogFiles(LogDirectory).First();
                        if (latestFile != CurrentFile)
                        {
                            CurrentFile = latestFile;
                            filePosition = ReadFileFromPosition(CurrentFile, 0);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error while reading journal file {0}", CurrentFile);
                    filePosition = new FileInfo(CurrentFile).Length; // Skipping the 'poisoned' data
                }
        }

        private long ReadFileFromPosition(string file, long filePosition)
        {
            using (var fileReader = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var textReader = new StreamReader(fileReader))
                {
                    fileReader.Position = filePosition;
                    var events = ReadEventsFromStream(textReader);
                    foreach (var @event in events)
                        basicObservable.OnNext(@event);
                    return fileReader.Position;
                }
            }
        }

        #region IDisposable Support
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
        #endregion

        public IDisposable Subscribe(IObserver<LogEvent> observer) => basicObservable.Subscribe(observer);
    }
}
