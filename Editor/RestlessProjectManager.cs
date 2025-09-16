using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using RestlessLib.SaveUtility;

namespace RestlessEditor
{
    [InitializeOnLoad]
    static class RestlessProjectManager
    {
        public static string ProjectName { get; private set; }
        public static string ProjectVersion { get; private set; }
        public static string UnityVersion { get; private set; }
        public static Texture2D ProjectIcon { get; private set; }
        public static double TotalProjectWorkhours { get; private set; }
        private class SavedData { public double total_project_workhours; }
        private static SavedData savedData = new SavedData();
        public static float Workhours { get; private set; }
        public static float CurrentSessionWorkhours { get; private set; }
        public static string BuildsPath { get; private set; }
        public static string LatestBuildName { get; private set; }

        public static List<PackageInfo> TrackedPackages = new List<PackageInfo>();

        // Time tracking
        private const string TotalTimeKey = "WorkTimeTracker_TotalTime";
        static DateTime lastTimecheck;

        static private float refreshInterval = 60f; // 60 seconds
        static private float lastRefreshTime = 0f;
        public static void Refresh()
        {
            ProjectName = PlayerSettings.productName;
            ProjectVersion = PlayerSettings.bundleVersion;
            UnityVersion = Application.unityVersion;
            BuildsPath = BuildPostProcessor.GetLastBuildPath();
            LatestBuildName = BuildPostProcessor.GetLastBuildName();
            ProjectIcon = GetDefaultIcon();

            // Time tracking
            TotalProjectWorkhours = savedData.total_project_workhours;
            TotalProjectWorkhours += (DateTime.Now - lastTimecheck).TotalHours;

            Workhours = EditorPrefs.GetFloat(TotalTimeKey, 0f);
            Workhours += (float)(DateTime.Now - lastTimecheck).TotalHours;

            CurrentSessionWorkhours = (float)EditorApplication.timeSinceStartup / 3600f;

            lastTimecheck = DateTime.Now;
            savedData.total_project_workhours = TotalProjectWorkhours;

            EditorPrefs.SetFloat(TotalTimeKey, Workhours);
            SaveUtility.Save(savedData, "project_workhours.json", false);

        }

        private static void Update()
        {
            if (EditorApplication.timeSinceStartup - lastRefreshTime > refreshInterval)
            {
                Refresh();
                lastRefreshTime = (float)EditorApplication.timeSinceStartup;
            }
        }

        private static void OnEditorQuit()
        {
            Refresh();
        }

        static RestlessProjectManager()
        {
            ProjectName = PlayerSettings.productName;
            ProjectVersion = PlayerSettings.bundleVersion;
            UnityVersion = Application.unityVersion;
            ProjectIcon = GetDefaultIcon();
            BuildsPath = BuildPostProcessor.GetLastBuildPath();
            LatestBuildName = BuildPostProcessor.GetLastBuildName();

            savedData = new SavedData();
            SaveUtility.CreateOrLoad(savedData, "project_workhours.json", verbose: false);
            TotalProjectWorkhours = savedData.total_project_workhours;

            Workhours = EditorPrefs.GetFloat(TotalTimeKey, 0f);
            CurrentSessionWorkhours = 0f;
            lastTimecheck = DateTime.Now;


            EditorApplication.update += Update;
            EditorApplication.quitting += OnEditorQuit;

            PackageInfo RestlessEnginePackage = new PackageInfo
            {
                Name = "com.restless.engine",
                DisplayName = "Restless Engine",
                pathToAssetsPackage = "",
                giturl = ""
            };
            PackageInfo RestlessLibPackage = new PackageInfo
            {
                Name = "com.restless.lib",
                DisplayName = "Restless Lib",
                pathToAssetsPackage = "Assets/Project/com.restless.lib",
                giturl = ""
            };
            PackageInfo RestlessUiPackage = new PackageInfo
            {
                Name = "com.restless.ui",
                DisplayName = "Restless UI",
                pathToAssetsPackage = "Assets/Project/com.restless.ui",
                giturl = ""
            };
            TrackedPackages.Add(RestlessEnginePackage);
            TrackedPackages.Add(RestlessLibPackage);
            TrackedPackages.Add(RestlessUiPackage);

            foreach (var pkg in TrackedPackages)
            {
                pkg.Refresh();
            }
        }
        public static void AddTrackedPackage(PackageInfo package)
        {
            TrackedPackages.Add(package);
        }
        private static Texture2D GetDefaultIcon()
        {
            var settingsAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (settingsAsset == null || settingsAsset.Length == 0)
                return null;

            var playerSettings = new SerializedObject(settingsAsset[0]);
            var iconsProp = playerSettings.FindProperty("m_BuildTargetIcons");

            if (iconsProp != null && iconsProp.arraySize > 0)
            {
                var entry = iconsProp.GetArrayElementAtIndex(0); // pierwszy = Default Icon
                var iconsArray = entry.FindPropertyRelative("m_Icons");

                if (iconsArray != null && iconsArray.arraySize > 0)
                {
                    var iconEntry = iconsArray.GetArrayElementAtIndex(0);
                    var texProp = iconEntry.FindPropertyRelative("m_Icon"); // <-- ważne!
                    return texProp.objectReferenceValue as Texture2D;
                }
            }
            return Texture2D.blackTexture;
        }


    }

    public class BuildPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            // Ścieżka do zbudowanego pliku / folderu
            string path = report.summary.outputPath;
            string buildName = report.name;

            // Zapisz np. w EditorPrefs
            Debug.Log("Last build at: " + path + " with name: " + buildName);

            EditorPrefs.SetString("LastBuildPath", path);
            EditorPrefs.SetString("LastBuildName", buildName);
        }
        public static string GetLastBuildPath()
        {
            return EditorPrefs.GetString("LastBuildPath", string.Empty);
        }

        public static string GetLastBuildName()
        {
            return EditorPrefs.GetString("LastBuildName", string.Empty);
        }
    }
}
