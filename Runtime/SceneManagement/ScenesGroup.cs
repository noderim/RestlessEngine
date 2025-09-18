using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using RestlessLib.Attributes;


namespace RestlessEngine.SceneManagement
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "Scenes Group", menuName = "RestlessEngine/Scenes/Scenes Group")]
    public class ScenesGroup : ScriptableObject
    {
        [Expandable]
        public List<SceneObject> Scenes = new List<SceneObject>();

        public async Task LoadScenesGroup()
        {
            foreach (var scene in Scenes)
            {
                await scene.LoadScene();
            }
        }
#if UNITY_EDITOR
        public void EditorOpenScenes()
        {
            foreach (var scene in Scenes)
            {
                ScenesSystem.EditorOpenScene(scene);
            }
        }


        [UnityEditor.CustomEditor(typeof(ScenesGroup))]
        public class ScenesGroupEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                ScenesGroup scenesGroup = (ScenesGroup)target;

                if (GUILayout.Button("Open Scenes"))
                {
                    LoadScenesGroupAsync(scenesGroup);
                }
            }

            private void LoadScenesGroupAsync(ScenesGroup scenesGroup)
            {
                scenesGroup.EditorOpenScenes();
            }
        }
#endif
    }
}
