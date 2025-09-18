using System;
using System.Collections.Generic;
using RestlessLib.Attributes;
using UnityEngine;

namespace RestlessEngine.Application.Settings
{
    [CreateAssetMenu(fileName = "Settings", menuName = "RestlessEngine/Settings/Settings")]
    public class SettingsData : ScriptableObject
    {
        [SerializeField]
        public GameplaySettings gameplaySettings;

        [HorizontalLine]
        [SerializeField]
        public AudioSettings audioSettings;

        [HorizontalLine]
        [SerializeField]
        public GraphicsSettings graphicsSettings;

        [HorizontalLine]
        [SerializeField]
        public ControlsSettings controlsSettings;
        [HorizontalLine]
        [SerializeField]
        public InterfaceSettings interfaceSettings;

        static private Dictionary<Type, Func<SettingsData, object>> getters =
            new Dictionary<Type, Func<SettingsData, object>>()
            {
                { typeof(GameplaySettings),  s => s.gameplaySettings },
                { typeof(AudioSettings),     s => s.audioSettings },
                { typeof(GraphicsSettings),  s => s.graphicsSettings },
                { typeof(ControlsSettings),  s => s.controlsSettings },
                { typeof(InterfaceSettings), s => s.interfaceSettings },
            };

        static private Dictionary<Type, Action<SettingsData, object>> setters = new Dictionary<Type, Action<SettingsData, object>>()
        {
            { typeof(GameplaySettings), (s, v) => s.gameplaySettings = (GameplaySettings)v },
            { typeof(AudioSettings),    (s, v) => s.audioSettings = (AudioSettings)v },
            { typeof(GraphicsSettings), (s, v) => s.graphicsSettings = (GraphicsSettings)v },
            { typeof(ControlsSettings), (s, v) => s.controlsSettings = (ControlsSettings)v },
            { typeof(InterfaceSettings), (s, v) => s.interfaceSettings = (InterfaceSettings)v },
        };


        public void CopyFrom(SettingsData other)
        {
            gameplaySettings = other.gameplaySettings;
            audioSettings = other.audioSettings;
            graphicsSettings = other.graphicsSettings;
            controlsSettings = other.controlsSettings;
            interfaceSettings = other.interfaceSettings;
        }

        public void CopyFrom<T>(SettingsData other) where T : struct
        {
            if (setters.TryGetValue(typeof(T), out var setter))
            {
                setter(this, other.Get<T>());
            }
            else
            {
                throw new InvalidOperationException($"Unsupported settings type: {typeof(T).Name}");
            }
        }


        public T Get<T>() where T : struct
        {
            if (getters.TryGetValue(typeof(T), out var getter))
                return (T)getter(this);

            throw new InvalidOperationException($"Unsupported settings type: {typeof(T).Name}");
        }

        public void Set<T>(T newSettings) where T : struct
        {
            if (setters.TryGetValue(typeof(T), out var setter))
            {
                setter(this, newSettings);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported settings type: {typeof(T).Name}");
            }
        }

    }
}
