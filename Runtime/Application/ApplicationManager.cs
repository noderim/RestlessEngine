using System.Collections.Generic;
using System.Threading.Tasks;
using RestlessEngine.Application.Runtime;
using RestlessEngine.Diagnostics;
using RestlessEngine.SceneManagement;
using RestlessLib.Attributes;
using UnityEngine;

namespace RestlessEngine.Application
{
    // APPLICATION ENTRY & END POINT
    [DisallowMultipleComponent]
    public class ApplicationManager : SingletonSystem<ApplicationManager>, IApplicationManager
    {
        [ReadOnly]
        [SerializeField]
        private AppState currentAppState = AppState.Initialization;
        public AppState CurrentAppState => currentAppState;

        [Header("Intro Splashscreen")]

        public bool SkipIntro = false;

        [SerializeField]
        SceneObject introScene;

        [Header("Main Menu")]
        [SerializeField]
        SceneObject mainMenuScene;
        private void Start()
        {
            _ = ApplicationStartupSequence();
        }
        private async Task ApplicationStartupSequence()
        {
            // Startup sequence
            // 1. Intro Sequance
            //   - Load Independent Intro Scene
            //   - Play Splashscreen Animations
            //   - Wait for user input or timeout
            //
            // 2. Initialization Sequance
            //   - Load Loading Screen Scene
            //   - Get all registered systems from SystemsRegistry
            //   - Initialize all registered systems via GameInitializationManager
            //
            // 3. Main Menu Sequance
            //   - After initialization Load Main Menu Scene
            //   - Main Menu Manager and Game Manager take over from here
            //   - On Main Menu Scene loaded startup sequence is complete

            LogManager.Log("Application Startup Sequence started.", LogTag.LifeCycle);

            // Intro Sequance
            if (SkipIntro == false)
            {
                LogManager.Log("Skip Intro disabled - Preceed Intro Splashscreen", LogTag.LifeCycle);
                currentAppState = AppState.Intro;
                await introScene.LoadScene();
                IntroSequenceController introControler = introScene.FindInScene<IntroSequenceController>();
                await introControler.StartSequence();
                await introScene.UnloadScene();
            }
            else
            {
                LogManager.Log("Skip Intro enabled - Skipping Intro Splashscreen", LogTag.LifeCycle);
            }

            // Initialization Sequance
            currentAppState = AppState.Initialization;
            LogManager.Log("Begin Initialization Sequence", LogTag.LifeCycle);

            LogManager.Log("Getting systems from Systems Registry");
            IReadOnlyList<SystemCore> systems = SystemsRegistry.GetRegisteredSystems();
            LogManager.Log($"{systems.Count} systems registered.", LogTag.LifeCycle);

            await GameInitializationManager.Instance.GameInitializationAsync(systems);

            // Main Menu Sequance

            await mainMenuScene.LoadScene();
            currentAppState = AppState.MainMenu;

            LogManager.Log("Startup Sequence complete", LogTag.LifeCycle);
        }
        private Task ApplicationQuitSequence()
        {
            LogManager.Log("Application Quit Sequence started.", LogTag.LifeCycle);
            LogManager.Log("Getting systems from Systems Registry");
            IReadOnlyList<SystemCore> systems = SystemsRegistry.GetRegisteredSystems();

            LogManager.Log($"Found {systems.Count} systems to shutdown.", LogTag.LifeCycle);

            //we shutdown in reverse order of initialization
            for (int i = systems.Count - 1; i >= 0; i--)
            {
                systems[i].Shutdown();
            }
            LogManager.Log("All systems shutdown.", LogTag.LifeCycle);
            Shutdown();
            return Task.CompletedTask;
        }
        private void OnApplicationQuit()
        {
            _ = ApplicationQuitSequence();
        }
        public void QuitGame()
        {
            UnityEngine.Application.Quit();
        }
    }

    public enum AppState
    {
        Initialization,
        Intro,
        MainMenu,
        Loading,
        Game,
    }
}
