using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using HashTool.Models;

namespace HashTool.Helpers
{
    internal class UpdateHelper
    {
        private static readonly HttpClient httpClient = new();

        public static async Task SetUpdate(UpdateModel update)
        {
            string owner = "KiyanYang";
            string repo = "HashTool";
            var realease = await GetRelease(owner, repo);
            if (realease != null)
            {
                update.HasUpdate = true;
                var ver = realease["tag_name"].GetString();
                if (ver != null)
                    update.Version = ver[1..];
                update.DownloadUrl = GetDownloadUrl(realease["assets"]);
                update.GithubUrl = $"https://github.com/KiyanYang/HashTool/releases/tag/{update.Version}";
                update.GiteeUrl = $"https://gitee.com/KiyanYang/HashTool/releases/{update.Version}";
            }
            else
            {
                update.HasUpdate = false;
            }
        }

        private static async Task<Dictionary<string, JsonElement>?> GetRelease(string owner, string repo)
        {
            string releasesUrl = $"https://gitee.com/api/v5/repos/{owner}/{repo}/releases?page=1&per_page=10&direction=desc";
            var result = await httpClient.GetAsync(releasesUrl);
            string responseBody = await result.Content.ReadAsStringAsync();
            var releases = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(responseBody);
            if (releases != null)
            {
                var ver = GetAssemblyVersion();
                //var ver = new Version(1,0,1);

                foreach (var i in releases)
                {
                    if (i["prerelease"].GetBoolean() == false)
                    {
                        var tag = i["tag_name"].GetString();
                        if (tag != null)
                        {
                            var v = new Version(tag[1..]);
                            if (v > ver)
                            {
                                return i;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static string GetDownloadUrl(JsonElement assets)
        {
            var res = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(assets);
            if (res == null)
                return string.Empty;

            JsonElement nameJsonElement;
            foreach (var i in res)
            {
                if (i.TryGetValue("name", out nameJsonElement))
                {
                    var name = nameJsonElement.GetString();
                    if (name != null
                        && Regex.Match(name, @"hashtool.*.zip", RegexOptions.IgnoreCase).Success)
                    {
                        var url = i["browser_download_url"].GetString();
                        if (url != null)
                        {
                            return url;
                        }
                    }
                }
            }

            return string.Empty;
        }

        public static Version GetAssemblyVersion()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            AssemblyName assemName = assem.GetName();
            var ver = assemName.Version;
            if (ver != null)
            {
                return ver;
            }

            return new Version();
        }
    }
}
