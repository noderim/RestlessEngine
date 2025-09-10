using RestlessEngine.Application.Settings;
using UnityEngine;
using UnityEngine.Events;


namespace RestlessEngine.Application.Runtime
{
    public class GameplaySystem : SingletonSystem<GameplaySystem>, ISettingsSystem
    {
        [SerializeField]
        public GameplaySettings gameplaySettings { get; private set; }
        UnityEvent ISettingsSystem.onSettingsApply { get; set; }

        public UnityEvent onSettingsApply;
        public void ApplySettings()
        {
            onSettingsApply?.Invoke();
        }

        public void GetSettings()
        {
            gameplaySettings = SettingsManager.Instance.GetCurrentSettings<GameplaySettings>();
        }

        public void LoadSettings(SettingsData settings)
        {
            gameplaySettings = settings.gameplaySettings;
        }
    }
}
