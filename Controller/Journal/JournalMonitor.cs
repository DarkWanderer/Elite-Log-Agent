using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DW.ELA.Interfaces;
using DW.ELA.Utility.App;
using NLog;
using NLog.Fluent;
using DW.ELA.Utility.Observable;

namespace DW.ELA.Controller
{
    /// <summary>
    /// This class runs in background to monitor and notify consumers (observers) of new log events
    /// </summary>
    public class JournalMonitor : JournalFileReader, ILogRealTimeDataSource
    {
        // Static/readonly members
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly FileSystemWatcher fileWatcher;
        private readonly string logDirectory;
        private readonly object @lock = new object();
        private readonly Timer logFlushTimer = new Timer();
        private readonly BasicObservable<JournalEvent> basicObservable = new BasicObservable<JournalEvent>();
        private readonly IReadOnlyCollection<string> eventsToReadFromFile = new HashSet<string> { "Outfitting", "Market", "Shipyard", "Cargo" };
        private readonly ConcurrentQueue<JournalEvent> queuedEvents = new ConcurrentQueue<JournalEvent>();
        private readonly TimeSpan checkInterval;

        // 
        private string currentFile;
        private long filePosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="JournalMonitor"/> class.
        ///
        /// </summary>
        /// <param name="logDirectoryProvider">Log directory name provider</param>
        /// <param name="checkInterval">Check interval in milliseconds</param>
        public JournalMonitor(ILogDirectoryNameProvider logDirectoryProvider, int checkIntervalMilliseconds = 10000)
        {
            checkInterval = TimeSpan.FromMilliseconds(checkIntervalMilliseconds);
            logDirectory = logDirectoryProvider.Directory;
            Directory.CreateDirectory(logDirectory); // In case Elite Dangerous was not launched yet
            fileWatcher = new FileSystemWatcher(logDirectory);

            fileWatcher.Changed += FileWatcher_Event;
            fileWatcher.Created += FileWatcher_Event;
            fileWatcher.NotifyFilter = NotifyFilters.CreationTime |
                                       NotifyFilters.FileName |
                                       NotifyFilters.LastWrite |
                                       NotifyFilters.Size;

            logFlushTimer.AutoReset = true;
            logFlushTimer.Interval = checkInterval.TotalMilliseconds; // sometimes the filesystem event does not trigger
            logFlushTimer.Elapsed += LogFlushTimer_Event;
            logFlushTimer.Enabled = true;

            currentFile = JournalFileEnumerator.GetLogFiles(logDirectory).FirstOrDefault();
            filePosition = string.IsNullOrEmpty(currentFile) ? 0 : new FileInfo(currentFile).Length;

            SendEventsFromJournal(false);
            fileWatcher.EnableRaisingEvents = true;
            Log.Info().Message("Started monitoring").Property("directory", logDirectory).Write();
        }

        private void LogFlushTimer_Event(object sender, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(() => SendEventsFromJournal(false));
            if (EliteDangerous.IsRunning)
                logFlushTimer.Interval = checkInterval.TotalMilliseconds;
            else
                logFlushTimer.Interval = checkInterval.TotalMilliseconds * 6;
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
                Log.Error()
                    .Message("Error while reading event file")
                    .Exception(e)
                    .Property("file", fullPath)
                    .Write();
            }
        }

        private void SendEventsFromJournal(bool checkOtherFiles)
        {
            lock (@lock)
            {
                try
                {
                    // We are not checking file size to make decision about whether
                    // we should read the file. Reason being - the log write operations
                    // are often buffered, so we need to open the file to flush buffers
                    if (currentFile != null)
                        filePosition = ReadJournalFromPosition(currentFile, filePosition);

                    if (checkOtherFiles || currentFile == null)
                    {
                        string latestFile = JournalFileEnumerator.GetLogFiles(logDirectory).FirstOrDefault();
                        if (latestFile == currentFile || latestFile == null)
                            return;

                        Log.Info()
                            .Message("Switched to new file")
                            .Property("file", latestFile)
                            .Write();

                        currentFile = latestFile;
                        filePosition = ReadJournalFromPosition(currentFile, 0);
                    }
                }
                catch (FileNotFoundException e)
                {
                    Log.Error()
                        .Message("Journal file not found")
                        .Exception(e)
                        .Property("file", currentFile)
                        .Write();
                    currentFile = null;
                }
                catch (Exception e)
                {
                    Log.Error()
                        .Message("Error while reading journal file")
                        .Exception(e)
                        .Property("file", currentFile)
                        .Write();
                }
            }
        }

        private long ReadJournalFromPosition(string file, long filePosition)
        {
            using var fileReader = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var textReader = new StreamReader(fileReader);
            try
            {
                fileReader.Position = filePosition;
                var events = ReadEventsFromStream(textReader);
                foreach (var @event in events)
                {
                    // Outfitting, market, etc. events are just indicators that data must be read from json
                    if (eventsToReadFromFile.Contains(@event.Event))
                        SendEventFromFile(GetJsonFileFullPath(@event.Event));
                    else
                        basicObservable.OnNext(@event);
                }
            }
            catch (Exception e)
            {
                Log.Error()
                    .Message("Error while reading journal file")
                    .Exception(e)
                    .Property("file", currentFile)
                    .Property("position", fileReader.Position)
                    .Write();
                textReader.ReadLine(); // read to end of current line, to skip 'bad' data
            }
            return fileReader.Position;
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

        public IDisposable Subscribe(IObserver<JournalEvent> observer) => basicObservable.Subscribe(observer);
    }
}
