// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System.Net.Http;
using Newtonsoft.Json;

#nullable enable

namespace osu.Game.Rulesets.oCrs.Online;

public class GitHubRelease
{
    [JsonProperty("html_url")]
    public string? htmlUrl { get; set; }

    [JsonProperty("tag_name")]
    public string? tagName { get; set; }
}

public class GetGitHubRelease : JsonWebRequestExtended<GitHubRelease>
{
    protected const string URL = "https://api.github.com/repos/fuyukiSmkw/osu-challengers-ruleset/releases/latest";

    public GetGitHubRelease()
        : base(URL)
    {
        Method = HttpMethod.Get;
        // ContentType = "application/json";
        // AddHeader("Accept", "application/json");
    }
}
