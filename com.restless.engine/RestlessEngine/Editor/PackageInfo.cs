using UnityEditor.PackageManager;
using UnityEngine;

namespace RestlessEditor
{
    public class PackageInfo
    {
        public string Name;
        public string DisplayName;
        public string Version;
        public string LatestVersion;
        public string pathToAssetsPackage;
        public string giturl;
        public bool InProject;

        public void Refresh()
        {
            var package = UnityEditor.PackageManager.PackageInfo.FindForPackageName(Name);
            if (package != null)
            {
                Version = package.version;
                DisplayName = package.displayName;
                giturl = package.repository.url;
                LatestVersion = package.versions.latest;
                InProject = true;

            }
            else
            {
                Version = "Not installed";
                InProject = false;
            }
        }
        public void GetInfofromPackageFile(string packageJsonPath)
        {
            if (System.IO.File.Exists(packageJsonPath))
            {
                var json = System.IO.File.ReadAllText(packageJsonPath);
                var package = JsonUtility.FromJson<PackageInfo>(json);
                if (package != null)
                {
                    Name = package.Name;
                    DisplayName = package.DisplayName;
                    Version = package.Version;
                    LatestVersion = package.LatestVersion;
                    giturl = package.giturl;
                }
            }
        }
        public void UpdatePackage()
        {

        }
        public void OpenInGitHub()
        {
            Application.OpenURL(giturl);
        }
        public void ImportAssets()
        {
            // Import the package assets
            var packagePath = System.IO.Path.GetDirectoryName(pathToAssetsPackage);
            if (!string.IsNullOrEmpty(packagePath))
            {
                UnityEditor.AssetDatabase.ImportPackage(packagePath, true);
            }
        }
    }
}
