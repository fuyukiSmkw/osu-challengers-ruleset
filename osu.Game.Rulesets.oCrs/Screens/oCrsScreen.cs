// Copyright (c) EVAST9919. Licensed under the MIT Licence.
// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Screens.Play;

namespace osu.Game.Rulesets.oCrs.Screens;

public abstract partial class oCrsScreen : ScreenWithBeatmapBackground
{
    protected override void LoadComplete()
    {
        base.LoadComplete();

        Beatmap.BindValueChanged(b => updateComponentFromBeatmap(b.NewValue));
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);
        this.FadeInFromZero(250, Easing.OutQuint);
        updateComponentFromBeatmap(Beatmap.Value);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOut(250, Easing.OutQuint);
        return base.OnExiting(e);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);
        this.FadeIn(250, Easing.OutQuint);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        base.OnSuspending(e);
        this.FadeOut(250, Easing.OutQuint);
    }

    private void updateComponentFromBeatmap(WorkingBeatmap beatmap)
    {
        ApplyToBackground(b =>
        {
            b.IgnoreUserSettings.Value = false;
            b.Beatmap = beatmap;
        });
    }
}
