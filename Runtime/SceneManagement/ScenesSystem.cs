using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestlessLib.Attributes;
using System;
using RestlessEngine.Diagnostics;

namespace RestlessEngine.SceneManagement
{
    [AddComponentMenu("Restless Engine/Systems/Scenes System")]
    public class ScenesSystem : SingletonSystem<ScenesSystem>
    {
        [Expandable]
        [SerializeField]
        public List<SceneObject> LoadedScenes;

        public async Task LoadSceneAsync(SceneObject sceneObject)
        {
            if (sceneObject == null)
            {
                LogManager.LogError("SceneObject is null.", LogTag.ApplicationSystem);
                return;
            }

            try
            {
                LogManager.Log($"Loading {sceneObject.LiteralSceneName} scene...", LogTag.ApplicationSystem);
                sceneObject.State = SceneState.Loading;

                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneObject.LiteralSceneName, LoadSceneMode.Additive);

                while (!asyncLoad.isDone)
                {
                    LogManager.Log($"{sceneObject.LiteralSceneName} loading {math.round(asyncLoad.progress * 100)}%", LogTag.Debug);
                    sceneObject.LoadingProgress = asyncLoad.progress;
                    await Task.Yield(); // Let Unity continue processing frames
                }

                sceneObject.State = SceneState.Loaded;
                LogManager.Log($"{sceneObject.LiteralSceneName} loaded.", LogTag.ApplicationSystem);

                LoadedScenes.Add(sceneObject);
            }
            catch (Exception e)
            {
                LogManager.LogError($"Error loading scene - {sceneObject.LiteralSceneName}: {e.Message}", LogTag.ApplicationSystem);
            }
        }

        public async Task UnloadSceneAsync(SceneObject sceneObject)
        {
            if (sceneObject == null)
            {
                LogManager.LogError("SceneObject is null.", LogTag.ApplicationSystem);
                return;
            }

            try
            {
                LogManager.Log($"Unloading {sceneObject.LiteralSceneName} scene...", LogTag.ApplicationSystem);
                string sceneName = SceneManager.GetSceneByName(sceneObject.LiteralSceneName).name;
                sceneObject.State = SceneState.Unloading;

                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

                while (!asyncUnload.isDone)
                {
                    LogManager.Log($"{sceneName} unloading {math.round(asyncUnload.progress * 100)}%", LogTag.Debug);
                    await Task.Yield();
                }

                sceneObject.State = SceneState.Unloaded;
                LogManager.Log($"{sceneName} unloaded.", LogTag.ApplicationSystem);

                LoadedScenes.Remove(sceneObject);
            }
            catch (Exception e)
            {
                LogManager.LogError($"Error unloading scene - {sceneObject.LiteralSceneName}: {e.Message}", LogTag.ApplicationSystem);
            }
        }
#if UNITY_EDITOR
        public static void EditorOpenScene(SceneObject sceneObject)
        {
            try
            {
                sceneObject.State = SceneState.Loading;

                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(sceneObject.GetPath(), UnityEditor.SceneManagement.OpenSceneMode.Additive);

                sceneObject.State = SceneState.Loaded;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading scene - {sceneObject.LiteralSceneName}: {e.Message}");
            }
        }
#endif
        public bool SelfValidation()
        {
            return true; // Currently no validation needed for this system.
        }
    }
}
