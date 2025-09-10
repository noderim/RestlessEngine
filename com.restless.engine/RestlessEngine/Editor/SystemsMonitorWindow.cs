namespace RestlessEditor
{
    using UnityEngine;
    using UnityEditor;
    using RestlessEngine.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;
    using RestlessEngine;
    using UnityEngine.UI;

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
                GUI.FocusControl(null); // Remove focus from the text field
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

            // foreach (var context in contexts)
            // {
            //     DrawSystemCard(context);
            // }



            EditorGUILayout.EndScrollView();

        }

        private void DrawSystemListMultiColumn(List<SystemMonitorContext> contexts)
        {
            int columns = 1; // domyślnie
            if (position.width > 750) columns = 2;
            if (position.width > 1100) columns = 3;

            int currentColumn = 0;

            EditorGUILayout.BeginHorizontal();
            foreach (var ctx in contexts)
            {
                // Każda karta ma ExpandWidth = false, stała szerokość np. 200
                GUILayout.BeginVertical(GUILayout.Width(position.width / columns - 8));
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

            GUI.color = bgcolor;
            // ramka karty
            using (new EditorGUILayout.HorizontalScope(boxStyle))
            {
                // LEWA: nazwa + parametry (flex)
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                {
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField(context.SystemName, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Initialized: " + context.IsInitialized);
                    EditorGUILayout.LabelField("Validated: " + context.Validated);
                    EditorGUILayout.HelpBox(string.IsNullOrEmpty(context.message) ? "No messages." : context.message, MessageType.None);
                }

                // PRAWA: 120 px szerokości, środek w pionie
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(120)))
                {
                    Rect rect = GUILayoutUtility.GetRect(120, 40, GUILayout.ExpandHeight(false)); // minimalna wysokość 40
                    float midY = rect.y + rect.height / 2f;

                    // dioda po prawej
                    Rect diodeRect = new Rect(rect.xMax - 16, midY - 6, 12, 12);
                    DrawIndicator(diodeRect, GetStateColor(context.State));

                    // tekst po lewej, ale align = Right -> napis będzie przylegał do diody
                    GUIStyle stateStyle = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleRight,
                        normal = { textColor = GetStateColor(context.State) }
                    };
                    EditorGUI.LabelField(new Rect(rect.x, midY - 8, rect.width - 20, 16), context.State.ToString(), stateStyle);
                }


            }
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