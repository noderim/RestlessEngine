using UnityEngine.Events;

namespace RestlessEngine.Application.Settings
{
    public interface ISettingsSystem
    {
        void ApplySettings();
        void GetSettings();
        void LoadSettings(SettingsData settings);
        public UnityEvent onSettingsApply { get; set; }
    }
}
