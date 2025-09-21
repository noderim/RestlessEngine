using System.Threading.Tasks;
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
        public bool FetchLatestFromGit = false;

        public async Task Refresh()
        {
            var package = UnityEditor.PackageManager.PackageInfo.FindForPackageName(Name);
            if (package != null)
            {
                Version = package.version;
                DisplayName = package.displayName;
                giturl = package.repository.url;
                pathToAssetsPackage = RestlessLib.Editor.PackageAssetsImporter.GetAssetsPathFromPackageJson(Name);
                if (FetchLatestFromGit)
                {
                    LatestVersion = await GitVersionFetcher.FetchLatestVersionFromGit(giturl);
                }
                else
                {
                    LatestVersion = package.versions.latest;
                }

                InProject = true;

            }
            else
            {
                Version = "Not installed";
                InProject = false;
                if (FetchLatestFromGit)
                {
                    LatestVersion = await GitVersionFetcher.FetchLatestVersionFromGit(giturl);
                }
            }
        }
        public void OpenInGitHub()
        {
            Application.OpenURL(giturl);
        }
    }
}
