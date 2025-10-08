using System;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using RestlessEngine.SceneManagement;

namespace RestlessEngine.GameSaves
{
    [Serializable]
    public class SaveFile
    {
        public int slot;
        public string savefileName;
        public string version = "0.0.0";
        [SerializeField]
        public SerializableDateTime lastSaved;
        public float playTimeSeconds;
        public bool isAutosave;
        [SerializedDictionary("Key", "Saved Json")]
        public SerializedDictionary<string, string> data;
        public SceneObject onloadScene;

        public bool Compare(SaveMetadata metadata)
        {
            if (metadata == null) return false;
            if (metadata.slot != slot) return false;
            if (metadata.savefileName != savefileName) return false;
            if (metadata.version != version) return false;
            if (metadata.lastSaved.Value != lastSaved.Value) return false;
            if (metadata.playTimeSeconds != playTimeSeconds) return false;
            if (metadata.isAutosave != isAutosave) return false;
            return true;
        }
        [System.Serializable]
        public class SerializableDateTime
        {
            [SerializeField]
            [HideInInspector]
            private long ticks;
            [SerializeField]
            public string date = "";

            public DateTime Value
            {
                get => new DateTime(ticks);
                set => ticks = value.Ticks;
            }

            public SerializableDateTime(DateTime dateTime)
            {
                ticks = dateTime.Ticks;
                date = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            public SerializableDateTime()
            {
                ticks = DateTime.Now.Ticks;
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            public override string ToString()
            {
                return Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }


    }
}
