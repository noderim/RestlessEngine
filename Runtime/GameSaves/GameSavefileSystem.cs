using System.Collections.Generic;
using UnityEngine;
using RestlessLib.Attributes;
using AYellowpaper.SerializedCollections;
using RestlessLib.SaveUtility;
using RestlessEngine.Diagnostics;

namespace RestlessEngine.GameSaves
{
    public class GameSavefileSystem : SingletonSystem<GameSavefileSystem>
    {
        [Header("Informations")]
        [ReadOnly]
        public bool HasSaveFiles;
        [ReadOnly]
        public int CurrentISaveables;
        [ReadOnly]
        public int CurrentSaves;
        [SerializeField]
        public SaveFile currentSaveFile;
        [HorizontalLine]
        [Expandable]
        [Header("Saves Metadata")]
        public SavesIndexer savesMetadata;
        private List<ISaveable> saveables = new List<ISaveable>();

        protected override bool Init()
        {
            LoadSavesMetadata();
            return true;
        }

        [ContextMenu("Save Game")]
        public void SaveGameManual()
        {
            //create a new save file
            SaveFile saveFile = new SaveFile
            {
                slot = savesMetadata.getNewSaveSlot(),
                savefileName = "SaveFile",
                version = UnityEngine.Application.version,
                lastSaved = new SaveFile.SerializableDateTime(),
                data = CaptureISaveables(),
                //playtime add,
                isAutosave = false,
            };
            savesMetadata.AddSave(saveFile);
            CurrentSaves = savesMetadata.saves.Count;

            SaveUtility.Save(savesMetadata);
            SaveUtility.Save(saveFile, $"SaveFiles/{saveFile.slot}_{saveFile.savefileName}.save");

            LogManager.Log("Game saved");
        }
        [ContextMenu("Save Game")]
        public void SaveGameManual(int slotOverride)
        {
            // TO-DO make separate panel controller
            // PromptManager.Instance.ShowPrompt(OverrideSavePrompt, () => SaveGameOverride(slotOverride));
        }

        public void SaveGameOverride(int slotOverride)
        {
            //create a new save file
            SaveFile saveFile = new SaveFile
            {
                slot = slotOverride,
                savefileName = "SaveFile",
                version = UnityEngine.Application.version,
                lastSaved = new SaveFile.SerializableDateTime(),
                data = CaptureISaveables(),
                //playtime add,
                isAutosave = false,
            };
            savesMetadata.AddSave(saveFile);
            CurrentSaves = savesMetadata.saves.Count;

            SaveUtility.Save(savesMetadata);
            SaveUtility.Save(saveFile, $"SaveFiles/{saveFile.slot}_{saveFile.savefileName}.save");

            LogManager.Log("Game saved");
        }
        public SerializedDictionary<string, string> CaptureISaveables()
        {
            var data = new SerializedDictionary<string, string>();
            // Save all ISaveable systems
            foreach (var saveable in saveables)
            {
                if (saveable != null)
                {
                    object obj = saveable.SaveState();
                    data.Add(saveable.GetUniqueID(), JsonUtility.ToJson(obj));
                }
            }

            return data;
        }
        public void RestoreISaveables(SerializedDictionary<string, string> data)
        {
            // Restore all ISaveable systems
            foreach (var saveable in saveables)
            {
                string id = saveable.GetUniqueID();
                if (data.ContainsKey(id))
                {
                    saveable.RestoreState(JsonUtility.FromJson(data[id], saveable.GetStateType()));
                }
            }
        }
        public void LoadSavesMetadata()
        {
            if (SaveUtility.Load(savesMetadata)) { HasSaveFiles = true; CurrentSaves = savesMetadata.saves.Count; }
            else { HasSaveFiles = false; CurrentSaves = 0; }
        }
        public string GetLastSaveInfo()
        {
            if (savesMetadata.saves.Count > 0)
            {

                SaveMetadata save = savesMetadata.saves[savesMetadata.saves.Count - 1];
                return $"{save.slot}_{save.savefileName} {save.lastSaved.ToString()}";
            }
            else
            {
                return "";
            }
        }
        public void AddSaveable(ISaveable saveable)
        {
            if (!saveables.Contains(saveable))
            {
                saveables.Add(saveable);
                CurrentISaveables = saveables.Count;
            }
        }
        public void LoadSave(SaveMetadata saveMetadata)
        {
            // Load the save file using the slot number
            SaveUtility.Load(currentSaveFile, $"SaveFiles/{saveMetadata.slot}_{saveMetadata.savefileName}.save");

            // Restore the state of all ISaveable systems
            RestoreISaveables(currentSaveFile.data);

            if (currentSaveFile.Compare(saveMetadata))
            {
                LogManager.Log("Game loaded successfully.", LogTag.ApplicationSystem);
            }
            else
            {
                LogManager.LogError("Failed to load save file. Metadata does not match.", LogTag.ApplicationSystem);
            }
        }
        public bool SelfValidation()
        {
            throw new System.NotImplementedException();
        }
    }
}
