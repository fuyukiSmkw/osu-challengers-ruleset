// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.oCrs.Graphics;

public partial class OcIcon : CompositeDrawable
{
    public OcIcon()
    {
        InternalChildren =
        [
            new SpriteIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(20f),
                Icon = FontAwesome.Regular.Circle,
                Colour = Color4.White,
            },
            new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Y = -0.5f,
                Text = "o!C",
                Font = OsuFont.Torus.With(size: 10, weight: FontWeight.SemiBold),
                Colour = Color4.White,
            }
        ];
    }
}
