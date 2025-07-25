// Copyright (c) 2025 MATRIX-feather. Licensed under the MIT Licence.
// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Overlays;
using osuTK.Graphics;

namespace osu.Game.Rulesets.oCrs.Graphics;

public partial class CustomColourProvider : OverlayColourProvider
{
    public Color4 ActiveColor => Highlight1;
    public Color4 InActiveColor => Dark4;

    public CustomColourProvider()
        : base(OverlayColourScheme.Pink)
    {
    }

    public CustomColourProvider(int hue)
        : base(hue)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        // Accent color
        UpdateHueColor(new Color4(222, 222, 222, 255f));
    }

    // 出于兼容性保留
    public BindableFloat HueColour = new();

    public void UpdateHueColor(Color4 color)
    {
        float hue = Color4.ToHsl(color).X * 360f;

        ChangeColourScheme((int)hue);
        HueColour.Value = hue;
    }
}
