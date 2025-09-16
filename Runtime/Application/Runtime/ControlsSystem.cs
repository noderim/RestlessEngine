using RestlessEngine.Application.Settings;
using UnityEngine;
using UnityEngine.Events;


namespace RestlessEngine.Application.Runtime
{
    public class ControlsSystem : SingletonSystem<ControlsSystem>, ISettingsSystem
    {
        [SerializeField]
        public ControlsSettings controlsSettings { get; private set; }
        public UnityEvent onSettingsApply { get; set; }

        public void ApplySettings()
        {
            onSettingsApply?.Invoke();
        }

        public void GetSettings()
        {
            controlsSettings = SettingsManager.Instance.GetCurrentSettings().controlsSettings;
        }

        public void LoadSettings(SettingsData settings)
        {
            throw new System.NotImplementedException();
        }
    }
}
