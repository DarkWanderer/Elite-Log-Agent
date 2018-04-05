namespace TrayAgent
{
    interface IPersistentSettingsStorage
        {
            void Save(UploaderSettings settings);
            UploaderSettings Load();
        }
}