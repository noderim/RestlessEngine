using System;
using UnityEngine;

namespace RestlessEngine.Application.Settings
{
    [Serializable]
    public struct GameplaySettings
    {
        [Space(10)]
        public bool AutoSaveEnabled;
        [Range(1, 60)]
        public int AutoSaveIntervalMinutes;
        [Space(10)]
        public bool ShowHints;

        public GameplaySettings(GameplaySettings other)
        {
            AutoSaveEnabled = other.AutoSaveEnabled;
            AutoSaveIntervalMinutes = other.AutoSaveIntervalMinutes;
            ShowHints = other.ShowHints;
        }
    }
}
