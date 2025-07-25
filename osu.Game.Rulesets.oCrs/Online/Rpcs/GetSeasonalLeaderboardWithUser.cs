// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using Newtonsoft.Json;

#nullable enable

namespace osu.Game.Rulesets.oCrs.Online.Rpcs;

public class SeasonalLeaderboardItem
{
    [JsonProperty("position")]
    public int? position { get; set; }

    [JsonProperty("total_score")]
    public int? totalScore { get; set; }

    [JsonProperty("user_id")]
    public int? challengersId { get; set; }

    [JsonProperty("username")]
    public string? username { get; set; }

    [JsonProperty("percentile")]
    public double? percentile { get; set; }

    [JsonProperty("is_target_user")]
    public bool? isTargetUser { get; set; }

    [JsonProperty("country")]
    public string? country { get; set; }

    [JsonProperty("challenges_participated")]
    public int? challengesParticipated { get; set; }

    [JsonProperty("average_accuracy")]
    public double? averageAccuracy { get; set; }

    [JsonProperty("avatar_url")]
    public string? avatarUrl { get; set; }
}

public class GetSeasonalLeaderboardWithUser : CallRpc<SeasonalLeaderboardItem[]>
{
    public GetSeasonalLeaderboardWithUser(int challengersId)
    : base("get_season_leaderboard_with_user")
    {
        AddRaw($"{{\"user_id_param\":{challengersId}, \"season_id_param\": 1}}"); // TODO
        Failed += (e) =>
        {
            Logging.LogError(e, $"Error requesting GetSeasonalLeaderboardWithUser, challengersId is {challengersId}");
        };
    }
}
