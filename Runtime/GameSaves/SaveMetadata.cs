namespace RestlessEngine.GameSaves
{
    [System.Serializable]
    public class SaveMetadata
    {
        public int slot;
        public string savefileName;
        public string version;
        public SaveFile.SerializableDateTime lastSaved;
        public float playTimeSeconds;
        public bool isAutosave;

        public SaveMetadata(SaveFile saveFile)
        {
            slot = saveFile.slot;
            savefileName = saveFile.savefileName;
            version = saveFile.version;
            lastSaved = saveFile.lastSaved;
            playTimeSeconds = saveFile.playTimeSeconds;
            isAutosave = saveFile.isAutosave;
        }
    }
}
