using System.Collections.Generic;
using UnityEngine;

namespace RestlessEngine.GameSaves
{
    [CreateAssetMenu(fileName = "SavesIndexer", menuName = "Save System/Saves Indexer")]
    public class SavesIndexer : ScriptableObject
    {
        public List<SaveMetadata> saves = new List<SaveMetadata>();

        public void AddSave(SaveFile save)
        {
            SaveMetadata newSave = new SaveMetadata(save);
            saves.Add(newSave);
        }
        public int getNewSaveSlot()
        {
            int newSlot = 0;
            foreach (var save in saves)
            {
                if (save.slot > newSlot)
                {
                    newSlot = save.slot;
                }
            }
            return newSlot + 1;
        }
    }
}
