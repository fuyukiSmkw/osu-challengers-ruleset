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
using osu.Game.Overlays.Profile.Header;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.oCrs.Extensions;
using osu.Game.Rulesets.oCrs.ListenerLoader.Utils;
using osu.Game.Rulesets.oCrs.Online.Rpcs;
using osu.Game.Rulesets.oCrs.Screens.ChallengersProfile;

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

    public partial class HackedTopHeaderContainer : TopHeaderContainer
    {
        [Resolved]
        private OsuGame game { get; set; } = null!;

        [Resolved]
        private HackedUserProfileOverlay userProfileOverlay { get; set; } = null!;

        protected OsuGame Game => game;

        private GetUserIdFromOsuId? request;
        private MyButton button = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();
            var flow = this.FindInstance("flow") as FillFlowContainer ?? null!;
            flow.Add(button = new MyButton("Go to osu!Challengers profile")
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Width = 0.4f,
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
                    request = new(User?.Value?.User.Id ?? 0);
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

    // NOTE: HACK!!!! Please keep up to date with lazer code
    public partial class HackedProfileHeader : ProfileHeader
    {
        protected override Drawable CreateContent()
        {
            var detailHeaderContainerField = this.FindFieldInstance("detailHeaderContainer");
            var centreHeaderContainerField = this.FindFieldInstance("centreHeaderContainer");
            DetailHeaderContainer detailHeaderContainer = new DetailHeaderContainer
            {
                RelativeSizeAxes = Axes.X,
                User = { BindTarget = User },
            };
            CentreHeaderContainer centreHeaderContainer = new CentreHeaderContainer
            {
                RelativeSizeAxes = Axes.X,
                User = { BindTarget = User },
            };
            detailHeaderContainerField?.SetValue(this, detailHeaderContainer);
            centreHeaderContainerField?.SetValue(this, centreHeaderContainer);

            return new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Children =
                [
                    new HackedTopHeaderContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        User = { BindTarget = User },
                    },
                    new BannerHeaderContainer
                    {
                        User = { BindTarget = User },
                    },
                    new BadgeHeaderContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        User = { BindTarget = User },
                    },
                    detailHeaderContainer,
                    centreHeaderContainer,
                    new BottomHeaderContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        User = { BindTarget = User },
                    },
                ]
            };
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
        replaceOrCacheAs<HackedUserProfileOverlay>(Game.Dependencies as DependencyContainer ?? null!, hacked);
        replaceOrCacheAs<UserProfileOverlay>(Game.Dependencies as DependencyContainer ?? null!, hacked);

        Logging.Log("Successfully injected HackedUserProfileOverlay!");
    }

    // Copyright (c) cdwcgt cdwcgt@cdwcgt.top>. Licensed under the MIT License.
    private static void replaceOrCacheAs<T>(DependencyContainer container, T replacement)
        where T : class
    {
        var cached = container.FindInstance("cache") as Dictionary<CacheInfo, object> ?? null!;

        var cacheInfo = cached.FirstOrDefault(c => (c.Key.FindInstance("Type") as Type)! == typeof(T)).Key;

        if (cacheInfo.Equals(new CacheInfo()))
        {
            container.CacheAs<T>(replacement);
            return;
        }

        cached[cacheInfo] = replacement;
    }

}
