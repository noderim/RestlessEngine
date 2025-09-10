using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RestlessEditor
{

    [InitializeOnLoad]
    public static class CustomHierarchyIcons
    {
        private static bool _hierarchyHasFocus = false;

        private static bool _disableCustomIcons = false;

        private static EditorWindow _hierarchyEditorWindow;

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            _hierarchyHasFocus = false;
            _disableCustomIcons = false;

            if (_hierarchyEditorWindow == null)
            {
                _hierarchyEditorWindow = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));
            }
            if (_disableCustomIcons)
            {
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
                EditorApplication.update -= OnEditorUpdate;
            }

        }
        static CustomHierarchyIcons()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            EditorApplication.update += OnEditorUpdate;
        }

        private static Color GetBackgroundColor(bool isSelected, bool isHovering, bool hasFocus)
        {
            Color defaultColor = EditorGUIUtility.isProSkin ? new Color(0.2196f, 0.2196f, 0.2196f) : new Color(0.7843f, 0.7843f, 0.7843f);

            Color SelectedColor = EditorGUIUtility.isProSkin ? new Color(0.24f, 0.49f, 0.91f) : new Color(0.24f, 0.49f, 0.91f);
            Color SelectedUnfocusedColor = EditorGUIUtility.isProSkin ? new Color(0.19f, 0.19f, 0.19f) : new Color(0.19f, 0.19f, 0.19f);

            Color HoverColor = EditorGUIUtility.isProSkin ? new Color(0.27f, 0.27f, 0.27f) : new Color(0.27f, 0.27f, 0.27f);

            if (isSelected)
            {
                return hasFocus ? SelectedColor : SelectedUnfocusedColor;
            }
            if (isHovering)
            {
                return HoverColor;
            }
            return defaultColor;
        }

        private static void OnEditorUpdate()
        {
            if (_hierarchyEditorWindow == null)
            {
                _hierarchyEditorWindow = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));
            }

            _hierarchyHasFocus = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow == _hierarchyEditorWindow;
        }
        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null)
            {
                return;
            }

            // Skip prefabs
            if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj) != null)
            {
                return;
            }

            Component[] components = obj.GetComponents<Component>();
            if (components == null || components.Length == 0)
            {
                return;
            }

            // Ensure we skip the Transform component
            Component component = components.FirstOrDefault(c => !(c is Transform));
            if (component == null)
            {
                return;
            }

            Type type = component.GetType();
            GUIContent content = EditorGUIUtility.ObjectContent(component, type);
            content.text = null;
            content.tooltip = type.Name;
            if (content.image == null)
            {
                return;
            }

            bool isSelected = Selection.instanceIDs.Contains(instanceID);
            bool isHovering = selectionRect.Contains(Event.current.mousePosition);

            Color color = GetBackgroundColor(isSelected, isHovering, _hierarchyHasFocus);
            Rect backgroundRect = selectionRect;
            backgroundRect.width = 18.5f;
            EditorGUI.DrawRect(backgroundRect, color);
            EditorGUI.LabelField(selectionRect, content);
        }
    }

}