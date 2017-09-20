using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.Inara.LogUploader.Persistence
{
    interface IPersistentFileInfoStorage
    {
        void SetLatestSavedFile(string fileName);
        string GetLatestSavedFile();
    }
}
