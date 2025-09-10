using System.Collections.Generic;
using System.Threading.Tasks;
using RestlessEngine.Diagnostics;
using RestlessEngine.SceneManagement;
using RestlessLib.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace RestlessEngine.Application
{
    public class GameInitializationManager : SingletonSystem<GameInitializationManager>, IGameInitializationManager
    {
        [Header("Info")]
        [ReadOnly]
        [SerializeField]
        private bool InitializationComplete;
        bool IGameInitializationManager.InitializationComplete => InitializationComplete;

        [ReadOnly]
        public float InitializationProgress = 0f;
        [ReadOnly]
        public string InitializationState;

        [Header("Events")]
        public UnityEvent onGameInitializationFinished;
        public UnityEvent onGameInitializationProgressChanged;
        public UnityEvent onGameInitializationStarted;
        [Header("Simulate Initialization Time")]
        protected bool simulateInitializationTime;
        public float UpdateStateDelay = 0.5f; // s


        internal virtual async Task GameInitializationAsync(IReadOnlyList<SystemCore> systems)
        {
            if (InitializationComplete)
            {
                LogManager.LogWarning("Game Initialization already completed. Skipping.", LogTag.LifeCycle);
                return;
            }
            InitializationComplete = false;
            float initprogresspersystem = 1f / systems.Count;

            await InitializeLocalizationSettings();

            onGameInitializationStarted?.Invoke();

            await UpdateState(0f, "Starting Initialization...");

            foreach (var system in systems)
            {
                // debug check
                if (system.IsInitialized)
                {
                    LogManager.Log($"{system.name} is already initialized.", LogTag.LifeCycle);
                    continue;
                }
                system.Initialize();
                await UpdateState(InitializationProgress + initprogresspersystem, $"{system.name} Initialized.");
            }

            InitializationComplete = true;
            await UpdateState(1f, "Initialization Complete.");
        }
        protected virtual async Task UpdateState(float progress, string state)
        {
            InitializationProgress = progress;
            InitializationState = state;
            onGameInitializationProgressChanged?.Invoke();

            if (simulateInitializationTime)
            {
                await Task.Delay((int)UpdateStateDelay * 1000);
            }
        }
        protected virtual async Task InitializeLocalizationSettings()
        {
            await LocalizationSettings.InitializationOperation.Task;
            LogManager.Log("Localization Settings Initialized", LogTag.LifeCycle);
        }
    }
}
