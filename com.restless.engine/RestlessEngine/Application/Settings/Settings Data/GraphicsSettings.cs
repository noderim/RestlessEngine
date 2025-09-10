using System;
using RestlessLib.Attributes;
using UnityEngine;

namespace RestlessEngine.Application.Settings
{
    [Serializable]
    public struct GraphicsSettings
    {
        [Header("Display Settings")]
        public ResolutionSetting resolution;
        public FullScreenMode ScreenModecreen;
        public int VSync;
        [InfoBox("0 - Disable\n1 - Enable\n2 - Every Second frame")]
        [Space(20)]
        public int PreferredRefreshRate;
        [Header("Graphics Settings")]
        public QualityPreset Preset;
        public float Brightness;
    }
    public enum QualityPreset
    {
        Low = 0,
        High = 1
    }
    public enum ResolutionSetting
    {
        Res1920x1080 = 0,
        Res2560x1440 = 1,
        Res1600x900 = 2,
        Res3440x1440 = 3
    }
}
