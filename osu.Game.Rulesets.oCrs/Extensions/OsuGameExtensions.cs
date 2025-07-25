// Copyright (c) EVAST9919. Licensed under the MIT Licence.
// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Testing;
using osu.Game.Overlays;
using osu.Game.Screens;

namespace osu.Game.Rulesets.oCrs.Extensions;

public static class OsuGameExtensions
{
    public static oCrsRuleset GetRuleset(this DependencyContainer dependencies)
    {
        var rulesets = dependencies.Get<RulesetStore>().AvailableRulesets.Select(info => info.CreateInstance());
        return (oCrsRuleset)rulesets.FirstOrDefault(r => r is oCrsRuleset);
    }

    public static oCrsRuleset GetThisRuleset(this RulesetStore rulesetStore)
    {
        var rulesets = rulesetStore.AvailableRulesets.Select(info => info.CreateInstance());
        return (oCrsRuleset)rulesets.FirstOrDefault(r => r is oCrsRuleset);
    }

    public static OsuScreenStack GetScreenStack(this OsuGame game) => game.ChildrenOfType<OsuScreenStack>().FirstOrDefault();

    public static SettingsOverlay GetSettingsOverlay(this OsuGame game) => game.ChildrenOfType<SettingsOverlay>().FirstOrDefault();
}
