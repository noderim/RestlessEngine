using UnityEngine;
using System;

namespace RestlessEngine.Application.Settings
{
    [Serializable]
    public struct AudioSettings
    {
        public bool MuteAll;
        public bool MuteMusic;
        [Space(10)]
        [Range(0, 1)]
        public float MasterVolume;
        [Range(0, 1)]
        public float MusicVolume;
        [Range(0, 1)]
        public float SFXVolume;
        [Range(0, 1)]
        public float UIVolume;
        [Range(0, 1)]
        public float AmbientVolume;
        [Space(10)]
        public bool MuteWhenInBackground;
    }
}
