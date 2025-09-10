using UnityEngine;
using RestlessLib.Attributes;
using RestlessLib.SaveUtility;
using RestlessEngine.Diagnostics;
using System.Collections.Generic;
using System;
using RestlessEngine.Application.Runtime;

namespace RestlessEngine.Application.Settings
{
    public class SettingsManager : SingletonSystem<SettingsManager>, ISettingsManager
    {
        [SerializeField]
        [Expandable]
        [HorizontalLine()]
        private SettingsData settings;

        [SerializeField]
        private SettingsData LastSavedSettings;

        [Expandable]
        [HorizontalLine()]
        [InfoBox("This is the default settings data that will be loaded if no settings data is found on the device or some are invalid", InfoBoxType.Info)]
        [SerializeField]
        private SettingsData DefaultSettings;

        [Header("Subsettings Systems")]
        private GameplaySystem gameplaySystem;
        private AudioSystem audioSystem;
        private GraphicsSystem graphicsSystem;
        private ControlsSystem controlSystem;
        private InterfaceSystem interfaceSystem;
        private Dictionary<Type, ISettingsSystem> SystemSettingsTypeMap;

        public void ChangeSettings<T>(T newSettings) where T : struct
        {
            settings.Set(newSettings);

            if (SystemSettingsTypeMap.TryGetValue(typeof(T), out var system))
            {
                system.LoadSettings(settings);

                //If audio settings changed, we apply them immediately
                if (system is AudioSystem)
                {
                    system.ApplySettings();
                }
            }
            else
            {
                LogManager.LogWarning($"No system registered for settings type: {typeof(T).Name}", LogTag.ApplicationSystem);
            }
        }
        public void LoadSettings()
        {
            foreach (var system in SystemSettingsTypeMap.Values)
            {
                system.LoadSettings(settings);
            }
        }
        public void LoadSettings<T>() where T : struct
        {
            if (SystemSettingsTypeMap.TryGetValue(typeof(T), out var system))
                system.LoadSettings(settings);
            else
                LogManager.LogWarning($"No system registered for settings type: {typeof(T).Name}", LogTag.ApplicationSystem);
        }
        public void ApplySettings()
        {
            foreach (var system in SystemSettingsTypeMap.Values)
            {
                system.ApplySettings();
            }
        }
        public void ApplySettings<T>() where T : struct
        {
            if (SystemSettingsTypeMap.TryGetValue(typeof(T), out var system))
                system.ApplySettings();
            else
                LogManager.LogWarning($"No system registered for settings type: {typeof(T).Name}", LogTag.ApplicationSystem);
        }
        public void RevertSettings()
        {
            settings.CopyFrom(LastSavedSettings);
            LoadSettings();
            ApplySettings();
        }
        public void RevertSettings<T>() where T : struct
        {
            settings.CopyFrom<T>(LastSavedSettings);
            LoadSettings<T>();
            ApplySettings<T>();

        }
        public void ResetToDefault()
        {
            settings.CopyFrom(DefaultSettings);
            LoadSettings();
            ApplySettings();
        }
        public void ResetToDefault<T>() where T : struct
        {
            settings.CopyFrom<T>(DefaultSettings);
            LoadSettings<T>();
            ApplySettings<T>();
        }
        [ContextMenu("Manual Save Settings")]
        public void SaveSettings()
        {
            SaveUtility.Save(settings);
            LastSavedSettings.CopyFrom(settings);
        }

        protected override bool Init()
        {
            gameplaySystem = GameplaySystem.Instance;
            audioSystem = AudioSystem.Instance;
            graphicsSystem = GraphicsSystem.Instance;
            controlSystem = ControlsSystem.Instance;
            interfaceSystem = InterfaceSystem.Instance;

            SystemSettingsTypeMap = new Dictionary<Type, ISettingsSystem>
            {
                { typeof(GameplaySettings), gameplaySystem },
                { typeof(AudioSettings), audioSystem },
                { typeof(GraphicsSettings), graphicsSystem },
                { typeof(ControlsSettings), controlSystem },
                { typeof(InterfaceSettings), interfaceSystem }
            };

            LogManager.Log("Preceed to load settings", LogTag.LifeCycle);
            LoadSavedSettings();

            LoadSettings();
            ApplySettings();
            return true;
        }

        [ContextMenu("Manual Load Settings")]
        private void LoadSavedSettings()
        {
            if (SaveUtility.Load(settings))
            {
                LogManager.Log("Settings loaded successfully.", LogTag.LifeCycle);
                LastSavedSettings.CopyFrom(settings);
            }
            else
            {
                LogManager.LogWarning("Failed to load settings. Loading default settings.", LogTag.LifeCycle);
                settings.CopyFrom(DefaultSettings);
            }
        }

        public T GetCurrentSettings<T>() where T : struct
        {
            return settings.Get<T>();
        }
        public SettingsData GetCurrentSettings()
        {
            return settings;
        }
    }
}
