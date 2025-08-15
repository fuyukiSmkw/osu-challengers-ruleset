// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;

namespace osu.Game.Rulesets.oCrs.Screens.ChallengersProfile.Components;

public partial class UserStatsContainer : CompositeDrawable
{
    private FillFlowContainer contentContainer;

    public UserStatsContainer()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChildren =
        [
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Children =
                [
                    contentContainer = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Horizontal,
                        // Spacing = new Vector2(20, 0),
                        Padding = new MarginPadding { Horizontal = 10 },
                    }
                ],
            },
        ];
    }

    public void updateStats(ChallengersProfileStats stats)
    {
        (string, string)[] items;
        try
        {
            items =
            [
                ("Average Accuracy", $"{Math.Round(stats.userStats.averageAccuracy ?? 0, 2)}%"),
                ("Seasonal Rank", $"#{stats.seasonalStats.position}"),
                ("Percentile", $"{stats.seasonalStats.percentile}%"),
                ("Participation", $"{stats.seasonalStats.challengesParticipated}"),
                ("Total Score", $"{stats.userStats.totalScores}"),
            ];
        }
        catch (Exception)
        {
            items = [("No stats in this season!", "Play some osu!Challengers!")];
        }
        contentContainer.Children = [.. items.Select(i => createItem(i.Item1, i.Item2, 1f / items.Length))];
    }

    private static FillFlowContainer createItem(string name, string value, float relativeWidth) => new()
    {
        Direction = FillDirection.Vertical,
        AutoSizeAxes = Axes.Y,
        Width = relativeWidth,
        RelativeSizeAxes = Axes.X,
        Anchor = Anchor.TopLeft,
        Origin = Anchor.TopLeft,
        Children =
        [
            new OsuSpriteText
            {
                Text = name,
                Font = OsuFont.GetFont(size: 14, weight: FontWeight.Bold),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Padding = new MarginPadding { Top = 5, Bottom = 2 },
            },
            new OsuSpriteText
            {
                Text = value,
                Font = OsuFont.GetFont(size: 16),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Padding = new MarginPadding { Top = 2, Bottom = 5 },
            },
        ]
    };


}
