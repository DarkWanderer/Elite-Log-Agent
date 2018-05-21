using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Utility.Observable;
using Interfaces;
using NLog;
using System.Linq;

namespace Controller
{
    /// <summary>
    /// This class runs a background thread to monitor and notify consumers (observers) of new log lines
    /// </summary>
    public class JsonLogMonitor : AbstractObservable<JObject>, ILogRealTimeDataSource
    {
        private readonly FileSystemWatcher fileWatcher;
        private string CurrentFile;
        private readonly string LogDirectory;
        private readonly ILogger Log;
        private long filePosition;
        private object @lock = new object();

        public JsonLogMonitor(ILogDirectoryNameProvider logDirectoryProvider, ILogger log)
        {
            LogDirectory = logDirectoryProvider.Directory;
            fileWatcher = new FileSystemWatcher(LogDirectory);

            fileWatcher.Changed += FileWatcher_Changed;
            fileWatcher.Created += FileWatcher_Created;
            fileWatcher.NotifyFilter = NotifyFilters.CreationTime |
                                       NotifyFilters.FileName |
                                       NotifyFilters.LastWrite |
                                       NotifyFilters.Size;

            CurrentFile = LogEnumerator.GetLogFiles(LogDirectory)[0];
            Log = log;
            filePosition = new FileInfo(CurrentFile).Length;
            Update(false);
            fileWatcher.EnableRaisingEvents = true;
            Log.Debug("Started monitoring on folder {0}", LogDirectory);
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (@lock)
                Update(checkOtherFiles: true);
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) != ".log")
                return;
            lock (@lock)
                Update(checkOtherFiles: e.FullPath != CurrentFile);
        }

        private void Update(bool checkOtherFiles)
        {
            Log.Trace("Starting update");
            try
            {
                filePosition = ReadFileFromPosition(CurrentFile, filePosition);
                if (checkOtherFiles)
                {
                    var filesToScan = LogEnumerator.GetLogFiles(LogDirectory)
                        .Take(10) // max 10 'missed' files to upload
                        .TakeWhile(f => f != CurrentFile) // only until we meet our file
                        .Reverse() // back to chronological order
                        .ToArray();
                    foreach (var file in filesToScan)
                        filePosition = ReadFileFromPosition(file, filePosition);
                    CurrentFile = filesToScan.Last();
                }
            }
            catch (Exception e)
            {
                OnError(e);
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
                        var @object = (JObject)serializer.Deserialize(jsonReader);
                        OnNext(@object);
                        Log.Debug("Received event: {0}", @object.ToString());
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
