using UnityEditor;
using UnityEngine;
using RestlessLib.SaveUtility; // Twój namespace z SaveUtility

namespace RestlessEditor
{
    public class RestlessNotepadWindow : EditorWindow
    {
        private const string FileName = "RestlessNotepad.json";
        private NotepadData data = new NotepadData();
        private Vector2 scroll;
        private bool isEditing = false;

        [MenuItem("RestlessEngine/Notepad")]
        public static void ShowWindow()
        {
            var window = GetWindow<RestlessNotepadWindow>("RestlessEngine Notepad");
            window.minSize = new Vector2(400, 300);
            window.LoadNote();
            window.Show();
        }

        private void OnEnable()
        {
            LoadNote();
        }

        private void OnGUI()
        {
            GUILayout.Space(8);
            EditorGUILayout.LabelField("RestlessEngine Notepad", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh", GUILayout.Height(25)))
            {
                LoadNote();
            }

            if (GUILayout.Button(isEditing ? "View" : "Edit", GUILayout.Height(25)))
            {
                isEditing = !isEditing;
            }

            if (GUILayout.Button("Save", GUILayout.Height(25)))
            {
                SaveNote();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            scroll = EditorGUILayout.BeginScrollView(scroll);

            if (isEditing)
            {
                data.content = EditorGUILayout.TextArea(data.content, GUILayout.ExpandHeight(true));
            }
            else
            {
                EditorGUILayout.HelpBox(string.IsNullOrEmpty(data.content) ? "(empty)" : data.content, MessageType.None);
            }

            EditorGUILayout.EndScrollView();
        }

        private void LoadNote()
        {
            var tempData = new NotepadData();
            if (SaveUtility.Load(tempData, FileName))
            {
                data = tempData;
            }
            else
            {
                data = new NotepadData();
            }
        }

        private void SaveNote()
        {
            SaveUtility.Save(data, FileName);
            AssetDatabase.Refresh();
        }

        [System.Serializable]
        private class NotepadData
        {
            public string content = string.Empty;
        }
    }
}
