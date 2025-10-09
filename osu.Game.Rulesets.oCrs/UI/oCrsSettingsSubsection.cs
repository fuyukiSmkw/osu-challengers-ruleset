// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.oCrs.Configuration;

#nullable enable

namespace osu.Game.Rulesets.oCrs.UI;

public partial class oCrsSettingsSubsection : RulesetSettingsSubsection
{
    private readonly Ruleset ruleset;

    protected override LocalisableString Header => ruleset.Description;

    public oCrsSettingsSubsection(Ruleset ruleset)
        : base(ruleset)
    {
        this.ruleset = ruleset;
    }

    private Bindable<bool> enableNoti = null!;
    private SettingsCheckbox alwaysNotiCheckbox = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        var config = (oCrsRulesetConfigManager)Config;

        enableNoti = config.GetBindable<bool>(oCrsRulesetSettings.EnableNewChallengersPlaylistNotification);

        Children =
        [
            new SettingsCheckbox
            {
                LabelText = "Show a notification when new o!C playlist is live",
                Current = enableNoti,
                Keywords = ["show", "new", "osu!", "challengers", "o!c", "oc", "playlists", "live", "notifications"]
            },
            alwaysNotiCheckbox = new SettingsCheckbox
            {
                LabelText = "Always show currently live o!C playlist notification (even if you have already played it)",
                Current = config.GetBindable<bool>(oCrsRulesetSettings.EnableAlwaysShowNewChallengersNotification),
                Keywords = ["always", "show", "currently", "osu!", "challengers", "o!c", "oc", "playlists", "live", "notifications"]
            },
            /*new SettingsTextBox
            {
                LabelText = "Debug: ChallengersNotificationCountsJson",
                Current = config.GetBindable<string>(oCrsRulesetSettings.ChallengersNotificationCountsJson),
            },*/
        ];

        enableNoti.BindValueChanged(v => Scheduler.AddOnce(() => updateVisibility(v.NewValue)));
    }

    private void updateVisibility(bool v)
    {
        if (v)
            alwaysNotiCheckbox?.Show();
        else
            alwaysNotiCheckbox?.Hide();
    }
}
