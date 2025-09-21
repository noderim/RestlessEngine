namespace RestlessEditor
{
    using UnityEngine;
    using UnityEditor;
    using System;

    public class RestlessProjectManagerWindow : EditorWindow
    {
        private Vector2 scrollPos;

        private GUIStyle headerStyle = new GUIStyle();
        private GUIStyle smallheaderStyle = new GUIStyle();

        private Texture2D trelloIcon;
        private Texture2D githubIcon;
        private Texture2D docsIcon;

        [MenuItem("RestlessEngine/Project Manager")]
        public static void ShowWindow()
        {
            GetWindow<RestlessProjectManagerWindow>("Restless Project Manager");
        }

        void OnEnable()
        {
            //it will be a package so use AssetDatabase.LoadAssetAtPath with the package path
            trelloIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.restless.engine/Editor/Resources/trello_icon.png", typeof(Texture2D));
            githubIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.restless.engine/Editor/Resources/github_icon.png", typeof(Texture2D));
            docsIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.restless.engine/Editor/Resources/docs_icon.png", typeof(Texture2D));
        }
        private void GetStyles()
        {
            headerStyle = new GUIStyle(EditorStyles.largeLabel);
            headerStyle.fontSize = 18;
            headerStyle.normal.textColor = Color.white;
            headerStyle.alignment = TextAnchor.MiddleCenter;

            smallheaderStyle = new GUIStyle(headerStyle);
            smallheaderStyle.alignment = TextAnchor.MiddleLeft;
            smallheaderStyle.fontSize = 14;
        }

        private void OnGUI()
        {
            GetStyles();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            DrawHeader();
            DrawProjectInfo();
            EditorGUILayout.Space(15);
            DrawBuildSection();

            DrawSeperator();

            DrawPackageSection();
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            GUILayout.Space(10);
            GUILayout.Label("Restless Project Manager", headerStyle);
            GUILayout.Space(10);
            int buttonMaxWidth = 40;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle buttonstyle = new GUIStyle(GUI.skin.button);
            buttonstyle.fixedWidth = buttonMaxWidth;
            buttonstyle.fixedHeight = buttonMaxWidth;

            if (GUILayout.Button(trelloIcon, buttonstyle)) { Application.OpenURL("https://trello.com/b/FwOl8MRt/restless-engine"); }
            GUILayout.Space(5);
            if (GUILayout.Button(githubIcon, buttonstyle)) { Application.OpenURL("https://github.com/NodeNameA/Restless-Engine"); }
            GUILayout.Space(5);
            if (GUILayout.Button(docsIcon, buttonstyle)) { Application.OpenURL("https://trello.com/b/FwOl8MRt/restless-engine"); }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
        }

        private void DrawProjectInfo()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Project", smallheaderStyle);

            GUILayout.Label("Project Name: " + RestlessProjectManager.ProjectName);
            GUILayout.Label("Project Version: " + RestlessProjectManager.ProjectVersion);
            GUILayout.Label("Unity Version: " + RestlessProjectManager.UnityVersion);
            // workhours are written as a float, convert to human readable with minutes:
            GUILayout.Label("Project Workhours: " + Mathf.FloorToInt((float)RestlessProjectManager.TotalProjectWorkhours) + "h " + Mathf.FloorToInt((float)((RestlessProjectManager.TotalProjectWorkhours % 1) * 60)) + "m");
            GUILayout.Label("This Editor: " + Mathf.FloorToInt(RestlessProjectManager.Workhours) + "h " + Mathf.FloorToInt((RestlessProjectManager.Workhours % 1) * 60) + "m");
            GUILayout.Label("Current Session: " + Mathf.FloorToInt(RestlessProjectManager.CurrentSessionWorkhours) + "h " + Mathf.FloorToInt((RestlessProjectManager.CurrentSessionWorkhours % 1) * 60) + "m");
            EditorGUILayout.EndVertical();
            // Project Icon
            GUILayout.Box(RestlessProjectManager.ProjectIcon ?? Texture2D.whiteTexture, GUILayout.Width(90), GUILayout.Height(90));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        private void DrawBuildSection()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            GUIStyle wrapStyle = new GUIStyle(GUI.skin.label);
            wrapStyle.wordWrap = true;
            GUILayout.Label("Latest Build Name: " + RestlessProjectManager.LatestBuildName, wrapStyle, GUILayout.Width(150));
            //path has to be able to occupy few lines
            GUILayout.Label("Path: " + RestlessProjectManager.BuildsPath, wrapStyle, GUILayout.Width(200));
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("New Build", GUILayout.Height(40), GUILayout.Width(120)))
            {
                EditorWindow.GetWindow(typeof(BuildPlayerWindow));
            }
            EditorGUILayout.EndHorizontal();
        }


        private void DrawPackageSection()
        {
            GUILayout.Label("Packages", smallheaderStyle);

            EditorGUILayout.BeginVertical("box");
            foreach (var pkg in RestlessProjectManager.TrackedPackages)
            {
                DrawPackageEntry(pkg);
                DrawSeperator();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSeperator()
        {
            int space = 0;
            GUILayout.Space(space);
            GUI.color = Color.gray;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            GUILayout.Space(space);
        }
        private void BeginCentered(int maxHeight)
        {
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(maxHeight));
            GUILayout.FlexibleSpace();
        }
        private void EndCentered()
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        private void DrawPackageEntry(PackageInfo pkg)
        {
            int entryHeight = 60;
            int buttonwidth = 100;
            GUILayout.BeginHorizontal(GUILayout.Height(entryHeight));

            BeginCentered(entryHeight);
            GUILayout.Label(pkg.Name, GUILayout.Width(120));
            EndCentered();

            GUILayout.Space(5);

            BeginCentered(entryHeight);
            GUILayout.Label(pkg.Version, GUILayout.Width(80));
            EndCentered();

            GUILayout.Space(5);
            BeginCentered(entryHeight);
            GUILayout.Label(pkg.LatestVersion, GUILayout.Width(120));
            if (pkg.Version != pkg.LatestVersion)
            {
                GUILayout.Label("new version available!", GUILayout.Width(120));
            }
            EndCentered();

            BeginCentered(entryHeight);
            GUILayout.Space(5);
            if (GUILayout.Button("Open Repo", GUILayout.Width(buttonwidth)))
            {
                pkg.OpenInGitHub();
            }
            GUILayout.Space(5);
            if (string.IsNullOrEmpty(pkg.pathToAssetsPackage) == false)
            {
                if (GUILayout.Button("Import Assets", GUILayout.Width(buttonwidth)))
                {
                    Debug.Log($"Importing assets for {pkg.Name}");
                    RestlessLib.Editor.PackageAssetsImporter.ImportAssetsFromPackageJson(pkg.Name);
                }
            }
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Import Assets", GUILayout.Width(buttonwidth)))
                {
                    //just grafical
                }
                GUI.enabled = true;
            }
            EndCentered();
            EditorGUILayout.EndHorizontal();
        }
    }
}
