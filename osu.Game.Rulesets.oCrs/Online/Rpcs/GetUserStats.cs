// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using Newtonsoft.Json;

namespace osu.Game.Rulesets.oCrs.Online.Rpcs;

public class UserStats
{
    [JsonProperty("total_challenges")]
    public int? totalChallenges { get; set; }

    [JsonProperty("total_scores")]
    public int? totalScores { get; set; }

    [JsonProperty("best_rank")]
    public int? bestRank { get; set; }

    [JsonProperty("avg_accuracy")]
    public double? averageAccuracy { get; set; }
}

public class GetUserStats : CallRpc<UserStats[]>
{
    public GetUserStats(int challengersId)
    : base("get_user_stats")
    {
        AddRaw($"{{\"p_user_id\":{challengersId}}}");
        Failed += (e) =>
        {
            Logging.LogError(e, $"Error requesting GetUserStats, challengersId is {challengersId}");
        };
    }
}
