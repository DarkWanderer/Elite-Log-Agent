using System;
using System.Runtime.InteropServices;

namespace Controller
{
    public class SavedGamesDirectoryHelper
    {
        public static string Directory
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
    }
}
