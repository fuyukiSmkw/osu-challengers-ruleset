// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;

namespace osu.Game.Rulesets.oCrs.Screens.ChallengersProfile.Components;

public partial class UserStatsContainer : CompositeDrawable
{
    private FillFlowContainer contentContainer;

    private OsuSpriteText avgAccValueText, seasonalRankValueText, percentileValueText, participationValueText, totalScoreValueText;

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
                        Children =
                        [
                            createItem("Average Accuracy", "-", out avgAccValueText, 1f/5),
                            createItem("Seasonal Rank", "-", out seasonalRankValueText, 1f/5),
                            createItem("Top", "-", out percentileValueText, 1f/5),
                            createItem("Participation", "-", out participationValueText, 1f/5),
                            createItem("Total Score", "-", out totalScoreValueText, 1f/5),
                        ],
                    }
                ],
            },
        ];
    }

    public void updateStats(ChallengersProfileStats stats)
    {
        try
        {
            avgAccValueText.Text = $"{Math.Round(stats.userStats.averageAccuracy ?? 0, 2)}%";
            seasonalRankValueText.Text = $"#{stats.seasonalStats.position}";
            percentileValueText.Text = $"{Math.Round(100 - stats.seasonalStats.percentile ?? 0, 2)}%";
            participationValueText.Text = $"{stats.seasonalStats.challengesParticipated}";
            totalScoreValueText.Text = $"{stats.userStats.totalScores}";
        }
        catch (Exception)
        {
            contentContainer.Children =
            [
                createItem("No stats in this season!", "Play some osu!Challengers!", out _, 1f),
            ];
        }
    }

    private static FillFlowContainer createItem(string name, string value, out OsuSpriteText refToValue, float relativeWidth) => new()
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
            refToValue = new OsuSpriteText
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
