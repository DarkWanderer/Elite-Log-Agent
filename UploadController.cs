using DW.Inara.LogUploader.Inara;
using DW.Inara.LogUploader.Persistence;
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
        public UploadController(IPersistentFileInfoStorage fileInfoStorage)
        {
            this.fileInfoStorage = fileInfoStorage;
        }

        private UploaderSettings settings;
        public UploaderSettings Settings
        {
            get
            {
                //if (settings == null)
                //    throw new InvalidOperationException("Uploader settings not set");
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        public event EventHandler<LogUploadEventArgs> LogUploadSuccessful;
        public event EventHandler<LogUploadEventArgs> LogUploadFailed;

        public void UploadLatestFile(bool checkIfUploadedBefore)
        {
            var filePath = LatestLogFileName;
            var fileName = Path.GetFileName(filePath);

            if (checkIfUploadedBefore && fileInfoStorage?.GetLatestSavedFile() == fileName)
                return; // file has been uploaded before, nothing to do

            try
            {
                if (Settings == null)
                    throw new InvalidOperationException("INARA credentials not set");

                Uploader.UploadFile(filePath, Settings.InaraUsername, Settings.InaraPassword);
                fileInfoStorage?.SetLatestSavedFile(fileName);
                OnLogUploadSuccessful(fileName);
            }
            catch (Exception e)
            {
                OnLogUploadFailed(fileName, e);
            }
        }

        private void OnLogUploadFailed(string fileName, Exception e)
        {
            LogUploadFailed?.Invoke(this, new LogUploadEventArgs(fileName, e.Message));
        }

        private void OnLogUploadSuccessful(string fileName)
        {
            LogUploadSuccessful?.Invoke(this, new LogUploadEventArgs(fileName));
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
                int result = SHGetKnownFolderPath(new Guid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"), 0, new IntPtr(0), out IntPtr path);
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
        private IPersistentFileInfoStorage fileInfoStorage;

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