// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.oCrs.Online.Rpcs;

public class GetCurrentSeasonId : CallRpc<int?>
{
    public GetCurrentSeasonId()
    : base("get_current_season_id")
    {
        Failed += (e) =>
        {
            Logging.LogError(e, $"Error requesting GetCurrentSeasonId");
        };
    }
}
