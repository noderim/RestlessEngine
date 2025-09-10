using System;

namespace RestlessEngine.GameSaves
{
    public interface ISaveable
    {
        string GetUniqueID();
        object SaveState();
        void RestoreState(object state);
        Type GetStateType();
        void AddSelfToSaveableList();
    }
}
