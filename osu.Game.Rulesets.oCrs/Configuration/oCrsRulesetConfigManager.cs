// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

#nullable enable

namespace osu.Game.Rulesets.oCrs.Configuration;

public class oCrsRulesetConfigManager(SettingsStore? settings, RulesetInfo ruleset, int? variant = null) : RulesetConfigManager<oCrsRulesetSettings>(settings, ruleset, variant)
{
    protected override void InitialiseDefaults()
    {
        base.InitialiseDefaults();

        SetDefault(oCrsRulesetSettings.EnableNewChallengersPlaylistNotification, true);
        SetDefault(oCrsRulesetSettings.EnableAlwaysShowNewChallengersNotification, false);
        SetDefault(oCrsRulesetSettings.ChallengersNotificationCountsJson, "{}");
    }
}


public enum oCrsRulesetSettings
{
    EnableNewChallengersPlaylistNotification,
    EnableAlwaysShowNewChallengersNotification,
    ChallengersNotificationCountsJson,
}
