using DW.Inara.LogUploader.Inara;
using DW.Inara.LogUploader.Settings;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;

namespace DW.Inara.LogUploader
{
    internal class UploadController : IDisposable
    {
        public class LogUploadEventArgs : EventArgs
        {
            public LogUploadEventArgs(string fileName)
            {
                FileName = fileName;
            }

            public LogUploadEventArgs(string fileName, string error)
            {
                FileName = fileName;
                Error = error;
            }

            public string FileName { get; }
            public string Error { get; }
        }

        private UploaderSettings settings;
        public UploaderSettings Settings
        {
            get
            {
                if (settings == null)
                    throw new InvalidOperationException("Uploader settings not set");
                return settings;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                settings = value;
            }
        }

        public event EventHandler<LogUploadEventArgs> LogUploadSuccessful;
        public event EventHandler<LogUploadEventArgs> LogUploadFailed;

        public void UploadLatestFile()
        {
            var filePath = LatestLogFileName;
            try
            {
                Uploader.UploadFile(filePath, Settings.InaraUsername, Settings.InaraPassword);
                OnLogUploadSuccessful(filePath);
            }
            catch (Exception e)
            {
                OnLogUploadFailed(filePath, e);
            }
        }

        private void OnLogUploadFailed(string filePath, Exception e)
        {
            LogUploadFailed?.Invoke(this, new LogUploadEventArgs(Path.GetFileName(filePath), e.Message));
        }

        private void OnLogUploadSuccessful(string filePath)
        {
            LogUploadSuccessful?.Invoke(this, new LogUploadEventArgs(Path.GetFileName(filePath)));
        }

        private string LatestLogFileName
        {
            get
            {
                var savedGamesDirectoryInfo = new DirectoryInfo(SavedGamesDirectory);
                var latestFile = savedGamesDirectoryInfo.GetFiles().OrderByDescending(f => f.CreationTimeUtc).FirstOrDefault()?.FullName;
                return latestFile;
            }
        }

        private static string SavedGamesDirectory
        {
            get
            {
                IntPtr path;
                int result = SHGetKnownFolderPath(new Guid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"), 0, new IntPtr(0), out path);
                if (result >= 0)
                {
                    return Marshal.PtrToStringUni(path) + @"\Frontier Developments\Elite Dangerous";
                }
                else
                {
                    throw new ExternalException("Failed to find the saved games directory.", result);
                }
            }
        }

        [DllImport("Shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UploadController() {
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