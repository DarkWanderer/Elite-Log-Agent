using DW.Inara.LogUploader.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.Inara.LogUploader.Persistence
{
    interface IPersistentSettingsStorage
    {
        void Save(UploaderSettings settings);
        UploaderSettings Load();
    }
}
