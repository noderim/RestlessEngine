using RestlessEngine.Application.Settings;
using UnityEngine;
using UnityEngine.Events;

namespace RestlessEngine.Application.Runtime
{
    public class GraphicsSystem : SingletonSystem<GraphicsSystem>, ISettingsSystem
    {
        public GraphicsSettings graphicsSettings { get; private set; }

        public UnityEvent onSettingsApply { get; set; }

        [ContextMenu("Apply Settings")]
        public void ApplySettings()
        {
            // Apply Resolution & Fullscreen Mode
            Resolution res = GetResolution(graphicsSettings.resolution);
            RefreshRate rate = new RefreshRate();
            rate.denominator = 1;
            rate.numerator = (uint)graphicsSettings.PreferredRefreshRate;

            Screen.SetResolution(res.width, res.height, graphicsSettings.ScreenModecreen, rate);
            UnityEngine.Application.targetFrameRate = graphicsSettings.PreferredRefreshRate;

            Screen.brightness = graphicsSettings.Brightness;
            // Apply VSync
            QualitySettings.vSyncCount = graphicsSettings.VSync;
            QualitySettings.SetQualityLevel((int)graphicsSettings.Preset);

            onSettingsApply?.Invoke();
        }

        public void GetSettings()
        {
            graphicsSettings = SettingsManager.Instance.GetCurrentSettings().graphicsSettings;
        }

        public void LoadSettings(SettingsData settings)
        {
            graphicsSettings = settings.graphicsSettings;
        }

        private Resolution GetResolution(ResolutionSetting resolutionSetting)
        {
            Resolution res = new Resolution();
            switch (resolutionSetting)
            {
                case ResolutionSetting.Res1920x1080:
                    res.width = 1920;
                    res.height = 1080;
                    break;
                case ResolutionSetting.Res2560x1440:
                    res.width = 2560;
                    res.height = 1440;
                    break;
                case ResolutionSetting.Res1600x900:
                    res.width = 1600;
                    res.height = 900;
                    break;
                default:
                    res.width = 1920;
                    res.height = 1080;
                    break;
            }
            return res;
        }
    }
}
