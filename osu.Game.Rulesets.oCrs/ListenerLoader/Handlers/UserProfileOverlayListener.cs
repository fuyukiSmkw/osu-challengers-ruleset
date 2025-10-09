// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Overlays.Profile;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.oCrs.Extensions;
using osu.Game.Rulesets.oCrs.ListenerLoader.Utils;
using osu.Game.Rulesets.oCrs.Online.Rpcs;
using osu.Game.Rulesets.oCrs.Screens;

namespace osu.Game.Rulesets.oCrs.ListenerLoader.Handlers;

public partial class UserProfileOverlayListener : AbstractHandler
{
    private partial class MyButton : SettingsButton
    {
        public MyButton(string text)
        {
            Text = okText = text;
        }

        private bool noOCProfile = false;
        private string okText;
        public void loading()
        {
            Enabled.Value = false;
            Text = "Loading...";
        }
        public void loginFirst()
        {
            Enabled.Value = false;
            Text = "Login first!";
        }
        public void noProfile()
        {
            noOCProfile = true;
            Enabled.Value = false;
            Text = "User doesn't have an o!C profile...";
        }
        public void ok()
        {
            Enabled.Value = true;
            Text = okText;
        }
        public void reshow()
        {
            if (noOCProfile)
            {
                ok();
            }
        }
    }

    // NOTE: HACK!!!! Please keep up to date with lazer code
    public partial class HackedProfileHeader : ProfileHeader
    {
        [Resolved]
        private OsuGame game { get; set; } = null!;
        protected OsuGame Game => game;

        [Resolved]
        private HackedUserProfileOverlay userProfileOverlay { get; set; } = null!;

        private GetUserIdFromOsuId? request;
        private MyButton button = null!;

        public HackedProfileHeader()
        {
            TabControlContainer.Add(button = new MyButton("Go to osu!Challengers profile")
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Width = 0.3f,
                Height = 30,
                X = 30,
                Action = () =>
                {
                    void goToProfile(int challengersId)
                    {
                        var screenStack = Game.GetScreenStack();
                        screenStack?.Push(new ChallengersProfileScreen(User?.Value?.User ?? null!, challengersId));
                        userProfileOverlay.Hide();
                        button.ok();
                    }

                    request?.Abort();
                    request = new(User.Value?.User.Id ?? 0);
                    request.Finished += () => Schedule(() =>
                    {
                        int? cid = request.ResponseObject;
                        if (cid is not null)
                        {
                            goToProfile((int)cid);
                        }
                        else
                        {
                            button.noProfile();
                        }
                    });
                    request.Failed += (_) => button.ok();

                    button.loading();
                    request.PerformAsync();
                }
            });
        }
    }


    public partial class HackedUserProfileOverlay : UserProfileOverlay
    {
        protected override ProfileHeader CreateHeader() => new HackedProfileHeader();
    }


    [BackgroundDependencyLoader]
    private void load()
    {
        Schedule(waitAndReplace);
    }

    private void waitAndReplace()
    {
        // overlayContent: contains all overlays
        var overlayContentField = Game.FindFieldInstance("overlayContent");
        if (overlayContentField?.GetValue(Game) is not Container overlayContent)
        {
            Schedule(waitAndReplace);
            return;
        }

        // original object of type UserProfileOverlay from overlayContent
        if (overlayContent.FirstOrDefault(o => o.GetType() == typeof(UserProfileOverlay)) is not OverlayContainer original)
        {
            Schedule(waitAndReplace);
            return;
        }

        // userProfileOverlay: private member of OsuGame
        var userProfileOverlayField = Game.FindFieldInstance(typeof(UserProfileOverlay));
        if (userProfileOverlayField is null)
        {
            Schedule(waitAndReplace);
            return;
        }

        // loadComponentSingleFile: private method of OsuGame
        // var loadComponentSingleFileMethod = typeof(OsuGame).GetMethod("loadComponentSingleFile", BindingFlags.Instance | BindingFlags.NonPublic);
        // var loadComponentSingleFileGenericMethod = loadComponentSingleFileMethod?.MakeGenericMethod(typeof(UserProfileOverlay));
        var loadComponentSingleFile = Game.FindMethod("loadComponentSingleFile", typeof(UserProfileOverlay));
        if (loadComponentSingleFile is null)
        {
            Schedule(waitAndReplace);
            return;
        }

        // showOverlayAboveOthers: private method of OsuGame
        var showOverlayAboveOthers = Game.FindMethod("showOverlayAboveOthers", typeof(void));
        if (showOverlayAboveOthers is null)
        {
            Schedule(waitAndReplace);
            return;
        }

        // beatmapSetOverlay: private member of OsuGame, featuring in informationalOverlays
        if (Game.FindInstance("beatmapSetOverlay") is not BeatmapSetOverlay beatmapSetOverlay)
        {
            Schedule(waitAndReplace);
            return;
        }
        // chatOverlay: private member of OsuGame, featuring in singleDisplayOverlays
        if (Game.FindInstance("chatOverlay") is not ChatOverlay chatOverlay)
        {
            Logging.Log("UserProfileOverlayListener: chatOverlay nulls, bye");
            Schedule(waitAndReplace);
            return;
        }
        // news: private member of OsuGame, featuring in singleDisplayOverlays
        if (Game.FindInstance("news") is not NewsOverlay news)
        {
            Logging.Log("UserProfileOverlayListener: news nulls, bye");
            Schedule(waitAndReplace);
            return;
        }
        // dashboard: private member of OsuGame, featuring in singleDisplayOverlays
        if (Game.FindInstance("dashboard") is not DashboardOverlay dashboard)
        {
            Logging.Log("UserProfileOverlayListener: dashboard nulls, bye");
            Schedule(waitAndReplace);
            return;
        }
        // beatmapListing: private member of OsuGame, featuring in singleDisplayOverlays
        if (Game.FindInstance("beatmapListing") is not BeatmapListingOverlay beatmapListing)
        {
            Logging.Log("UserProfileOverlayListener: beatmapListing nulls, bye");
            Schedule(waitAndReplace);
            return;
        }
        // changelogOverlay: private member of OsuGame, featuring in singleDisplayOverlays
        if (Game.FindInstance("changelogOverlay") is not ChangelogOverlay changelogOverlay)
        {
            Logging.Log("UserProfileOverlayListener: changelogOverlay nulls, bye");
            Schedule(waitAndReplace);
            return;
        }
        // rankingsOverlay: featuring in singleDisplayOverlays
        // dep cache
        if ((Game.Dependencies as DependencyContainer)!.getFromCache<RankingsOverlay>() is not RankingsOverlay rankingsOverlay)
        {
            Logging.Log("UserProfileOverlayListener: rankingsOverlay nulls, bye");
            Schedule(waitAndReplace);
            return;
        }
        // wikiOverlay: private member of OsuGame, featuring in singleDisplayOverlays
        if (Game.FindInstance("wikiOverlay") is not WikiOverlay wikiOverlay)
        {
            Logging.Log("UserProfileOverlayListener: wikiOverlay nulls, bye");
            Schedule(waitAndReplace);
            return;
        }

        Logging.Log("UserProfileOverlayListener: nothing nulls, start injecting HackedUserProfileOverlay");

        var hacked = new HackedUserProfileOverlay
        {
            Depth = original.Depth,
        };

        var informationalOverlays = new OverlayContainer[] { beatmapSetOverlay, hacked };
        hacked.State.ValueChanged += state =>
        {
            if (state.NewValue != Visibility.Hidden)
                showOverlayAboveOthers([hacked, informationalOverlays]);
        };
        var singleDisplayOverlays = new OverlayContainer[] { chatOverlay, news, dashboard, beatmapListing, changelogOverlay, rankingsOverlay, wikiOverlay };
        foreach (var overlay in singleDisplayOverlays)
        {
            overlay.State.ValueChanged += state =>
            {
                // informational overlays should be dismissed on a show or hide of a full overlay.
                // informationalOverlays.ForEach(o => o.Hide());
                hacked.Hide();

                if (state.NewValue != Visibility.Hidden)
                    showOverlayAboveOthers([overlay, singleDisplayOverlays]);
            };
        }

        original.Expire();
        overlayContent.Remove(original, true);

        // overlayContent.Add(hacked);
        loadComponentSingleFile([
            hacked,
            (Action<Drawable>)overlayContent.Add,
            false
        ]);

        userProfileOverlayField?.SetValue(Game, hacked);

        // (Game.Dependencies as DependencyContainer)?.Cache(hacked);
        (Game.Dependencies as DependencyContainer)!.replaceOrCacheAs<HackedUserProfileOverlay>(hacked);
        (Game.Dependencies as DependencyContainer)!.replaceOrCacheAs<UserProfileOverlay>(hacked);

        Logging.Log("Successfully injected HackedUserProfileOverlay!");
    }

}
