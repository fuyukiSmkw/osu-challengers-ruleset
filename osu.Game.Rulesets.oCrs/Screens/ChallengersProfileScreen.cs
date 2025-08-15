// Copyright (c) EVAST9919. Licensed under the MIT Licence.
// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.oCrs.Extensions;
using osu.Game.Rulesets.oCrs.Online.Rpcs;
using osu.Game.Rulesets.oCrs.Screens.ChallengersProfile;
using osu.Game.Rulesets.oCrs.Screens.ChallengersProfile.Components;
using osu.Game.Rulesets.UI;
using osu.Game.Screens;
using osuTK;

#nullable enable

namespace osu.Game.Rulesets.oCrs.Screens;

public partial class ChallengersProfileScreen : oCrsScreen
{
    protected APIUser user;
    protected int challengersId;

    protected ChallengersProfileStats stats;

    private readonly LoadingLayer loadingLayer;

    private FillFlowContainer contentContainer;

    private SimpleUserHeaderContainer topHeader;
    private UserStatsContainer userStatsContainer;

    [Resolved]
    private IAPIProvider? api { get; set; }
    [Resolved]
    private RulesetStore? rulesets { get; set; }

    public ChallengersProfileScreen(APIUser user, int challengersId)
    {
        this.user = user;
        this.challengersId = challengersId;
        stats = new();

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children =
            [
                new BasicScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Direction = FillDirection.Vertical,
                        Width = 0.8f,
                        Padding = new MarginPadding { Top = 40 },
                        Spacing = new Vector2(0, 16),
                        Children =
                        [
                            topHeader = new SimpleUserHeaderContainer(user, challengersId),
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
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Direction = FillDirection.Vertical,
                                        Padding = new MarginPadding { Top = 40 },
                                        Spacing = new Vector2(0, 16),
                                        Children =
                                        [
                                            userStatsContainer = new UserStatsContainer(),
                                        ],
                                    },
                                    loadingLayer = new LoadingLayer(true),
                                ],
                            },
                        ],
                    },
                },
            ],
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Schedule(fetchAndSetContent);
    }

    private GetUserStats? userStatsRequest;
    private GetSeasonalLeaderboardWithUser? seasonalLeaderboardRequest;
    private GetCurrentSeasonId? seasonIdRequest;

    private async void fetchAndSetContent()
    {
        loadingLayer.Show();

        seasonIdRequest?.Abort();
        userStatsRequest?.Abort();
        seasonalLeaderboardRequest?.Abort();

        seasonIdRequest = new();
        seasonIdRequest.Finished += () => stats.seasonId = seasonIdRequest.ResponseObject ?? 0;

        userStatsRequest = new(challengersId);
        userStatsRequest.Finished += () => stats.userStats = userStatsRequest.ResponseObject[0];

        await seasonIdRequest.AwaitRequest();
        await userStatsRequest.AwaitRequest();

        seasonalLeaderboardRequest = new(challengersId, stats.seasonId);
        seasonalLeaderboardRequest.Finished += () =>
        {
            foreach (var item in seasonalLeaderboardRequest.ResponseObject)
            {
                if (item.isTargetUser ?? false)
                {
                    stats.seasonalStats = item;
                    break;
                }
            }
        };

        await seasonalLeaderboardRequest.AwaitRequest();

        userStatsContainer.updateStats(stats);

        /*var task1 = userStatsRequest.AwaitRequest();

        Task.WhenAll(task1).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                foreach (var ex in t.Exception.InnerExceptions)
                {
                    Logging.LogError(ex, "Request failed.");
                }
                return;
            }
            if (t.IsCanceled)
            {
                Logging.Log("Some tasks were cancelled.");
                return;
            }

            var userStats = task1.Result;

            Schedule(() =>
            {
                // TODO
            });
        });*/

        loadingLayer.Hide();
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        var baseDependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        return new OsuScreenDependencies(false, new DrawableRulesetDependencies(baseDependencies.GetRuleset(), baseDependencies));
    }
}
