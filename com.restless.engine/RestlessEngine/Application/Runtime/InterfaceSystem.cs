using System.Threading.Tasks;
using UnityEngine.Localization.Settings;
using UnityEngine;
using RestlessEngine.Application.Settings;
using RestlessEngine.Diagnostics;
using UnityEngine.Events;


namespace RestlessEngine
{
    public class InterfaceSystem : SingletonSystem<InterfaceSystem>, ISettingsSystem
    {

        [SerializeField]
        public InterfaceSettings interfaceSettings { get; private set; }
        public UnityEvent onSettingsApply { get; set; }

        public void ApplySettings()
        {
            switch (interfaceSettings.language)
            {
                case InterfaceSettings.Language.English:
                    SetLocaleAsync("en").Wait(2000); // Wait 5s for the task to complete
                    break;
                case InterfaceSettings.Language.Polish:
                    SetLocaleAsync("pl").Wait(2000); // Wait 5s for the task to complete
                    break;
                case InterfaceSettings.Language.French:
                    SetLocaleAsync("fr").Wait(2000); // Wait 5s for the task to complete
                    break;
                case InterfaceSettings.Language.Deutsch:
                    SetLocaleAsync("de").Wait(2000); // Wait 5s for the task to complete
                    break;
                case InterfaceSettings.Language.Spanish:
                    SetLocaleAsync("es").Wait(2000); // Wait 5s for the task to complete
                    break;

                default:
                    // Default settings
                    break;
            }

            onSettingsApply?.Invoke();
        }

        public async Task SetLocaleAsync(string localeCode)
        {
            LogManager.Log($"Setting locale to {localeCode}", LogTag.ApplicationSystem);
            // Wait for the localization system to initialize
            await LocalizationSettings.InitializationOperation.Task;

            // Get available locales
            var locales = LocalizationSettings.AvailableLocales.Locales;
            // Find and set the desired locale
            foreach (var locale in locales)
            {
                if (locale.Identifier.Code == localeCode)
                {
                    LogManager.Log($"Locale found: {localeCode}", LogTag.ApplicationSystem);
                    LocalizationSettings.SelectedLocale = locale;
                    break;
                }
            }
        }

        public void GetSettings()
        {
            interfaceSettings = SettingsManager.Instance.GetCurrentSettings<InterfaceSettings>();
        }

        public void LoadSettings(SettingsData settings)
        {
            interfaceSettings = settings.interfaceSettings;
        }
    }
}
