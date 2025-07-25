// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.oCrs.Graphics;
using osu.Game.Users;
using osu.Game.Users.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.oCrs.Screens.ChallengersProfile.Components;

public partial class SimpleUserHeaderContainer : CompositeDrawable
{
    private const float content_height = 47;
    private const float avatar_height = 100;
    private const float vertical_padding = 10;

    private readonly APIUser user;
    private readonly int challengersId;

    public SimpleUserHeaderContainer(APIUser user, int challengersId)
    {
        this.user = user;
        this.challengersId = challengersId;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChildren =
        [
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Children =
                [
                    new ProfileCoverBackground
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 250,
                        User = user,
                    },
                    new Container
                    {
                        // AutoSizeAxes = Axes.Both,
                        RelativeSizeAxes = Axes.X,
                        Height = content_height + 2 * vertical_padding,
                        Children =
                        [
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = new CustomColourProvider(user.ProfileHue ?? 333).Background3,
                            },
                            /*new FillFlowContainer
                            {
                                Name = "Gradient dim boxes",
                                RelativeSizeAxes = Axes.Both,
                                // AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,

                                Children =
                                [
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Width = 0.7f,
                                        Colour = Color4.Black,
                                    },
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Width = 0.1f,
                                        Colour = ColourInfo.GradientHorizontal(Color4.Black, Color4.Black.Opacity(0.666f)),
                                    },
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Width = 0.1f,
                                        Colour = ColourInfo.GradientHorizontal(Color4.Black.Opacity(0.666f), Color4.Black.Opacity(0.333f)),
                                    },
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Width = 0.1f,
                                        Colour = ColourInfo.GradientHorizontal(Color4.Black.Opacity(0.333f), Color4.Black.Opacity(0f)),
                                    }
                                ]
                            },*/
                            new FillFlowContainer
                            {
                                Direction = FillDirection.Horizontal,
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                                Padding = new MarginPadding
                                {
                                    Left = 30,
                                    Vertical = vertical_padding
                                },
                                Height = content_height + 2 * vertical_padding,
                                // RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Both,
                                Children =
                                [
                                    new UpdateableAvatar(user)
                                    {
                                        Anchor = Anchor.BottomLeft,
                                        Origin = Anchor.BottomLeft,
                                        Size = new Vector2(avatar_height),
                                        CornerRadius = 20,
                                        Masking = true,
                                        EdgeEffect = new EdgeEffectParameters
                                        {
                                            Type = EdgeEffectType.Shadow,
                                            Offset = new Vector2(0, 1),
                                            Radius = 3,
                                            Colour = Color4.Black.Opacity(0.25f),
                                        }
                                    },
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Anchor = Anchor.BottomLeft,
                                        Origin = Anchor.BottomLeft,
                                        Padding = new MarginPadding { Left = 10 },
                                        Children =
                                        [
                                            new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.Both,
                                                Direction = FillDirection.Horizontal,
                                                Spacing = new Vector2(5, 0),
                                                Children =
                                                [
                                                    new OsuSpriteText
                                                    {
                                                        Text = user.Username,
                                                        Font = OsuFont.GetFont(size: 24, weight: FontWeight.Regular),
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                    },
                                                    new ExternalLinkButton
                                                    {
                                                        Link = $"https://www.challengersnexus.com/profile/{challengersId}",
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                    },
                                                    new ChallengersBadge(user)
                                                    {
                                                        Width = 100.5f,
                                                        Height = 24,
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                    },
                                                ]
                                            },
                                            /*new OsuSpriteText
                                            {
                                                Text = user.Title ?? string.Empty,
                                                Font = OsuFont.GetFont(size: 16, weight: FontWeight.Regular),
                                                Margin = new MarginPadding { Bottom = 3 },
                                                Colour = Color4Extensions.FromHex(user.Colour ?? "FFFFFF")
                                            },*/
                                            new FillFlowContainer
                                            {
                                                Margin = new MarginPadding { Top = 3 },
                                                AutoSizeAxes = Axes.Both,
                                                Direction = FillDirection.Horizontal,
                                                Spacing = new Vector2(4, 0),
                                                Children =
                                                [
                                                    new UpdateableFlag(user.CountryCode)
                                                    {
                                                        Size = new Vector2(28, 20),
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                    },
                                                    new OsuSpriteText
                                                    {
                                                        Text = user.CountryCode.GetDescription(),
                                                        Font = OsuFont.GetFont(size: 14f),
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }
                        ],
                    },
                ]
            }
        ];
    }

    private partial class ProfileCoverBackground : UserCoverBackground
    {
        protected override double LoadDelay => 0;
        public ProfileCoverBackground() => Masking = true;
    }

}
