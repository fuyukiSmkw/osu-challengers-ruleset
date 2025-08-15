// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.oCrs.Online.Rpcs;

namespace osu.Game.Rulesets.oCrs.Screens.ChallengersProfile;

public struct ChallengersProfileStats
{
    public UserStats userStats;
    public SeasonalLeaderboardItem seasonalStats;
    public int seasonId;
}
