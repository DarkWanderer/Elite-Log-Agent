using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Utility.Observable;
using Interfaces;
using NLog;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using DW.ELA.LogModel;
using DW.ELA.Interfaces;

namespace Controller
{
    /// <summary>
    /// This class runs in background to monitor and notify consumers (observers) of new log lines
    /// </summary>
    public class JournalMonitor : AbstractObservable<LogEvent>, ILogRealTimeDataSource
    {
        private readonly FileSystemWatcher fileWatcher;
        private string CurrentFile;
        private readonly string LogDirectory;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private long filePosition;
        private object @lock = new object();
        private readonly Timer logFlushTimer = new Timer();

        public TimeSpan LogFlushInterval { set => logFlushTimer.Interval = value.TotalMilliseconds; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logDirectoryProvider">Log directory name provider</param>
        /// <param name="checkInterval">Check interval in milliseconds</param>
        public JournalMonitor(ILogDirectoryNameProvider logDirectoryProvider, int checkInterval = 5000)
        {
            LogDirectory = logDirectoryProvider.Directory;
            fileWatcher = new FileSystemWatcher(LogDirectory);

            fileWatcher.Changed += FileWatcher_Changed;
            fileWatcher.Created += FileWatcher_Created;
            fileWatcher.NotifyFilter = NotifyFilters.CreationTime |
                                       NotifyFilters.FileName |
                                       NotifyFilters.LastWrite |
                                       NotifyFilters.Size;

            logFlushTimer.AutoReset = true;
            logFlushTimer.Interval = checkInterval; // sometimes the filesystem event does not trigger
            logFlushTimer.Elapsed += (o, e) => Task.Factory.StartNew(() => Update(false));
            logFlushTimer.Enabled = true;

            CurrentFile = LogEnumerator.GetLogFiles(LogDirectory).First();
            filePosition = new FileInfo(CurrentFile).Length;
            Update(false);
            fileWatcher.EnableRaisingEvents = true;
            logger.Debug("Started monitoring on folder {0}", LogDirectory);
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Update(checkOtherFiles: true);
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) != ".log")
                return;
            Update(checkOtherFiles: e.FullPath != CurrentFile);
        }

        private void Update(bool checkOtherFiles)
        {
            lock (@lock)
                try
                {
                    filePosition = ReadFileFromPosition(CurrentFile, filePosition);
                    if (checkOtherFiles) {
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
                    logger.Error(e, "Error while reading log file, skipping");
                    filePosition = new FileInfo(CurrentFile).Length; // Skipping the 'poisoned' data
                }
        }

        private long ReadFileFromPosition(string file, long filePosition)
        {
            using (var fileReader = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fileReader.Position = filePosition;
                using (var textReader = new StreamReader(fileReader))
                using (var jsonReader = new JsonTextReader(textReader) { SupportMultipleContent = true })
                {
                    JsonSerializer serializer = new JsonSerializer();
                    while (jsonReader.Read())
                    {
                        var record = serializer.Deserialize(jsonReader);
                        // Sometimes serializer gives out the json as string instead of JObject
                        var @object = record as JObject ?? JObject.Parse(record as string);
                        OnNext(LogEventConverter.Convert(@object));
                        logger.Debug("Received event: {0}", @object.ToString());
                    }
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
                    fileWatcher.Changed -= FileWatcher_Changed;
                    fileWatcher.Created -= FileWatcher_Created;
                    fileWatcher.Dispose();
                    logFlushTimer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

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
    }
}
