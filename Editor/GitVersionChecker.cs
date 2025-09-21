using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static class GitVersionFetcher
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<string> FetchLatestVersionFromGit(string gitUrl, string branch = "main")
    {
        if (string.IsNullOrWhiteSpace(gitUrl) || !gitUrl.Contains("github.com"))
            return null;

        try
        {
            // Normalizujemy adres
            string cleanUrl = gitUrl.Replace(".git", "");
            string[] parts = cleanUrl.Split('/');
            if (parts.Length < 2)
                return null;

            string owner = parts[parts.Length - 2];
            string repo = parts[parts.Length - 1];

            // URL do raw package.json
            string packageJsonUrl = $"https://raw.githubusercontent.com/{owner}/{repo}/{branch}/package.json";

            // Pobieramy zawartość package.json
            string json = await client.GetStringAsync(packageJsonUrl);

            // Parsujemy JSON i wyciągamy wersję
            JObject obj = JObject.Parse(json);
            return obj["version"]?.ToString();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"[GitVersionFetcher] Error: {ex.Message}");
            return null;
        }
    }
}
