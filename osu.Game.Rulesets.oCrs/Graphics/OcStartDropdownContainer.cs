// Copyright (c) 2025 MATRIX-feather. Licensed under the MIT Licence.
// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.oCrs.Graphics;

public partial class OcStartDropdownContainer : BasicDropdownContainer
{
    protected override bool StartHidden { get; } = true;

    public FillFlowContainer ButtonContainer { get; private set; }

    public OcStartDropdownContainer()
    {
        var pad = new MarginPadding(12);
        pad.Top = pad.Bottom = 24;
        ButtonContainer = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
            // Padding = new MarginPadding(24),
            Padding = pad,
            // Direction = FillDirection.Full,
            Direction = FillDirection.Horizontal,

            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
        };
    }

    private Container backgroundImageContainer;
    private FillFlowContainer gradientFillFlow;
    private Box backgroundMask;
    private Box dimBox;

    protected override void OnColorUpdated()
    {
        base.OnColorUpdated();

        gradientFillFlow.Colour = ColourProvider.Background5;
        backgroundMask.Colour = ColourProvider.Background5.Opacity(0.9f);
    }

    private partial class HoverBox : Box
    {
        protected override bool OnHover(HoverEvent e)
        {
            this.FadeTo(0.8f, 500, Easing.OutQuint);
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.FadeTo(1, 500, Easing.OutQuint);
            base.OnHoverLost(e);
        }
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        // TextureStore textures = new TextureStore(renderer, new TextureLoaderStore(new oCrsRuleset().CreateResourceStore()), false);
        textures.AddTextureSource(new TextureLoaderStore(new oCrsRuleset().CreateResourceStore()));
        BackgroundContainer.Children =
        [
            backgroundImageContainer = new Container
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Masking = true,
                Width = 1300,
                Children =
                [
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        // Height = TargetHeight,
                        // Width = 1300,
                        Child = new Sprite
                        {
                            RelativeSizeAxes = Axes.Both,
                            Texture = textures.Get("Textures/bglong.png"),
                            FillMode = FillMode.Fill,
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                        },
                    },
                ],
            },
            gradientFillFlow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,

                Children =
                [
                    dimBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.2f,
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.1f,
                        Colour = ColourInfo.GradientHorizontal(Color4.White, Color4.White.Opacity(0.666f)),
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.1f,
                        Colour = ColourInfo.GradientHorizontal(Color4.White.Opacity(0.666f), Color4.White.Opacity(0.333f)),
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.1f,
                        Colour = ColourInfo.GradientHorizontal(Color4.White.Opacity(0.333f), Color4.White.Opacity(0f)),
                    }
                ]
            },
            backgroundMask = new HoverBox
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
            }
        ];

        ContentContainer.AddRange(
        [
            new OsuAnimatedButton
            {
                Size = new Vector2(18),
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding(24),
                Action = Hide,
                Children =
                [
                    new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.Times,
                        RelativeSizeAxes = Axes.Both,
                        Scale = new Vector2(0.8f),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                ]
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(24),
                Direction = FillDirection.Vertical,
                Children =
                [
                    new OsuSpriteText
                    {
                        Text = "osu!Challengers",
                        Font = OsuFont.Torus.With(size: 24, weight: FontWeight.SemiBold),
                    },
                    new OsuSpriteText
                    {
                        Text = "a collection of competitive events where you can test your skills, win supporter, and have fun with the community!",
                        Font = OsuFont.Torus.With(size: 12),
                    },
                ]
            },
            ButtonContainer
        ]);
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        ButtonContainer.Width = 1.0f;
    }
}
