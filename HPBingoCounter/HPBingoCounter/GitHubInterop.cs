using Microsoft.ClearScript.JavaScript;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter
{
    internal static class GitHubInterop
    {
        public const string ISSUES_URL = "https://github.com/nt5664/HPBingoCounter/issues";
        public const string USERGUIDE_URL = "https://github.com/nt5664/HPBingoCounter/wiki/User-Guide";
        public const string RELEASES_URL = "https://github.com/nt5664/HPBingoCounter/releases";

        public static async Task<bool> IsNewerVersionAvailableAsync(Version currentVersion)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("HPBingoCounter"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("nt5664", "HPBingoCounter");

            if (releases is null || releases.Count == 0)
                return false;

            Version? latestVersion = releases.Max(x => Version.Parse(x.TagName?.Replace("v", string.Empty) ?? "0.0.0"));
            if (latestVersion is null || latestVersion <= currentVersion)
                return false;

            return true;
        }
    }
}
