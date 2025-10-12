using UnityEngine;
using UnityEditor;
using RestlessEngine.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using RestlessEngine;

namespace RestlessEditor
{
    public class SystemsMonitorWindow : EditorWindow
    {
        private string searchQuery = "";
        private Vector2 scrollPos;

        [MenuItem("RestlessEngine/Systems Monitor")]
        public static void ShowWindow()
        {
            GetWindow<SystemsMonitorWindow>("Systems Monitor");
        }

        private void OnGUI()
        {
            if (SystemsMonitor.Instance == null)
            {
                EditorGUILayout.HelpBox("SystemsMonitor is not running in the scene.", MessageType.Warning);
                return;
            }

            // Search bar
            EditorGUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            GUILayout.Label("Search: ", GUILayout.Width(50));
            searchQuery = GUILayout.TextField(searchQuery);
            if (GUILayout.Button("X", GUI.skin.FindStyle("ToolbarButton"), GUILayout.Width(20)))
            {
                searchQuery = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Scrollable list
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            List<SystemMonitorContext> contexts = SystemsMonitor.Instance._contexts;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                contexts = contexts.Where(c =>
                    (c.SystemName != null && c.SystemName.ToLower().Contains(searchQuery.ToLower())) ||
                    c.State.ToString().ToLower().Contains(searchQuery.ToLower())
                ).ToList();
            }

            DrawSystemListMultiColumn(contexts);

            EditorGUILayout.EndScrollView();

        }

        private void DrawSystemListMultiColumn(List<SystemMonitorContext> contexts)
        {
            int columns = 1;
            if (position.width > 750) columns = 2;
            if (position.width > 1100) columns = 3;

            int currentColumn = 0;

            EditorGUILayout.BeginHorizontal();
            foreach (var ctx in contexts)
            {
                GUILayout.BeginVertical(GUILayout.MaxWidth(position.width / columns), GUILayout.ExpandWidth(true));
                DrawSystemCard(ctx);
                GUILayout.EndVertical();

                currentColumn++;
                if (currentColumn >= columns)
                {
                    currentColumn = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSystemCard(SystemMonitorContext context)
        {
            GUIStyle boxStyle = new GUIStyle();
            int margin = 4;
            int padding = 8;
            Color bgcolor = new Color(0.3f, 0.3f, 0.3f);

            Texture2D texture = new Texture2D(1, 1);
            texture.filterMode = FilterMode.Point;
            texture.SetPixel(0, 0, bgcolor);
            texture.Apply();

            boxStyle.normal.background = texture;
            boxStyle.normal.textColor = Color.black;
            boxStyle.margin = new RectOffset(margin, margin, margin, margin);
            boxStyle.padding = new RectOffset(padding, padding, padding, padding);

            GUILayout.BeginVertical(boxStyle);

            GUILayout.BeginHorizontal();

            // Left: system info
            GUILayout.BeginVertical();
            GUILayout.Label(context.SystemName, EditorStyles.boldLabel);
            GUILayout.Label($"Init Priority: {context.InitPriority}");
            GUILayout.Label($"Is Initialized: {context.IsInitialized}");
            GUILayout.Label($"Validated: {context.Validated}");
            GUILayout.EndVertical();

            // Right: state and indicator
            GUILayout.FlexibleSpace(); // pushes right section to edge
            GUILayout.BeginVertical(GUILayout.Width(120));
            GUILayout.BeginHorizontal();

            GUIStyle rightAligned = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight

            };
            rightAligned.normal.textColor = GetStateColor(context.State);
            GUILayout.Label($"{context.State}", rightAligned, GUILayout.Width(100));

            // Indicator circle
            Rect lastRect = GUILayoutUtility.GetRect(20, 20);
            DrawIndicator(lastRect, GetStateColor(context.State));

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            EditorGUILayout.HelpBox(string.IsNullOrEmpty(context.message) ? "No messages." : context.message, MessageType.None);
            GUILayout.EndVertical();
        }

        private void DrawIndicator(Rect rect, Color color)
        {
            Vector2 center = rect.center;
            Handles.color = color;
            Handles.DrawSolidDisc(center, Vector3.forward, 4f);
        }
        private Color GetStateColor(SystemState state)
        {
            return state switch
            {
                SystemState.Uninitialized => Color.gray,
                SystemState.Initializing => new Color(1f, 0.65f, 0f), // pomarańcz
                SystemState.Initialized => Color.cyan,
                SystemState.Running => Color.green,
                SystemState.Error => Color.red,
                SystemState.ShuttingDown => Color.yellow,
                _ => Color.white
            };
        }
    }

}