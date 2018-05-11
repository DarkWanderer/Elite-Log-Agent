﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Utility.Observable;

namespace Controller
{
    /// <summary>
    /// This class runs a background thread to monitor and notify consumers (observers) of new log lines
    /// </summary>
    public class JsonLogMonitor : AbstractObservable<JObject>
    {
        private FileSystemWatcher fileWatcher;
        private readonly string CurrentFile;
        private readonly string LogDirectory;
        private long filePosition;
        private object @lock = new object();

        public JsonLogMonitor(string logDirectory)
        {
            LogDirectory = logDirectory;
            fileWatcher = new FileSystemWatcher(LogDirectory);

            fileWatcher.Changed += FileWatcher_Changed;
            fileWatcher.Created += FileWatcher_Created;
            fileWatcher.NotifyFilter = NotifyFilters.Attributes |
                                        NotifyFilters.CreationTime |
                                        NotifyFilters.FileName |
                                        NotifyFilters.LastAccess |
                                        NotifyFilters.LastWrite |
                                        NotifyFilters.Size |
                                        NotifyFilters.Security;

            CurrentFile = LogEnumerator.GetLogFiles(LogDirectory)[0];
            filePosition = new FileInfo(CurrentFile).Length;
            Update(false);
            fileWatcher.EnableRaisingEvents = true;
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
            try
            {
                var file = CurrentFile;
                filePosition = ReadFileFromPosition(file, filePosition);
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
                    }
                    return fileReader.Position;
                }
            }
        }
    }
}
