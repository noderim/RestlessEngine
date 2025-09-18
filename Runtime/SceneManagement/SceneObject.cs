using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using RestlessLib.Attributes;
namespace RestlessEngine.SceneManagement
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SceneObject", menuName = "RestlessEngine/Scenes/SceneObject", order = 1)]
    public class SceneObject : ScriptableObject
    {
        [Scene]
        public string LiteralSceneName;
        [InfoBox("The name of the scene as it appears in the Build Settings.")]
        [Space(10)]
        [HorizontalLine]
        [Header("Scene Information")]
        public string GenericSceneName;
        [Space(10)]
        [Scene]
        public int SceneIndex;
        public SceneType Type;
        [Space(10)]
        public SceneState State;
        [ReadOnly]
        public float LoadingProgress;

        public async Task LoadScene()
        {
            if (IsLoaded()) return;
            await ScenesSystem.Instance.LoadSceneAsync(this);
        }
        public async Task UnloadScene()
        {
            if (IsLoaded()) await ScenesSystem.Instance.UnloadSceneAsync(this);
        }
        public T FindInScene<T>() where T : Component
        {
            if (!IsLoaded())
                return null;
            Scene scene = GetScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                T found = root.GetComponentInChildren<T>(true);
                if (found != null)
                    return found;
            }
            return null;
        }

        public Scene GetScene()
        {
            Scene scene = SceneManager.GetSceneByName(LiteralSceneName);
            return scene;
        }

        public string GetPath()
        {
            return SceneUtility.GetScenePathByBuildIndex(SceneIndex);
        }

        public bool IsLoaded()
        {
            Scene scene = SceneManager.GetSceneByName(LiteralSceneName);

            return scene.IsValid() && scene.isLoaded;
        }
    }

    public enum SceneState
    {
        Unloaded,
        Loaded,
        Loading,
        Unloading,
        Active,
        ExcludedFromBuild
    }
    public enum SceneType
    {
        UI,
        Systems,
        Level
    }
}
