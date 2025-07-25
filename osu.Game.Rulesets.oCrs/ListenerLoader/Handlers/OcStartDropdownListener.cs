// Copyright (c) 2025 MATRIX-feather. Licensed under the MIT Licence.
// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.oCrs.Extensions;
using osu.Game.Rulesets.oCrs.Graphics;
using osu.Game.Rulesets.oCrs.ListenerLoader.Utils;
using osu.Game.Rulesets.oCrs.Online.Rpcs;
using osu.Game.Rulesets.oCrs.Screens.ChallengersProfile;
using osu.Game.Screens.Menu;
using osu.Game.Screens.OnlinePlay.Lounge;
using osu.Game.Screens.OnlinePlay.Playlists;
using osuTK.Graphics;

#nullable enable

namespace osu.Game.Rulesets.oCrs.ListenerLoader.Handlers;

public partial class OcStartDropdownListener : AbstractHandler
{
    [Resolved(canBeNull: true)]
    private IBindable<RulesetInfo>? ruleset { get; set; }

    [Resolved]
    private RulesetStore? rulesets { get; set; }

    [Resolved]
    private Bindable<WorkingBeatmap> beatmap { get; set; } = null!;

    [Resolved]
    private IAPIProvider? api { get; set; }

    [Resolved]
    private CustomColourProvider colourProvider { get; set; } = null!;

    private BasicDropdownContainer? featureHeaderOverlay;

    private Box background = new() { RelativeSizeAxes = Axes.Both };

    [BackgroundDependencyLoader]
    private void load()
    {
        if (ruleset is not Bindable<RulesetInfo> rs) return;

        Container masterContainer;
        Game.Add(masterContainer = new Container
        {
            Name = "OcStartDropdownBackground",
            RelativeSizeAxes = Axes.Both,
            Alpha = 0,
            Children =
            [
                background,
            ]
        });

        LoadComponentAsync(createFeatureMenu(), loaded =>
        {
            loaded.State.BindValueChanged(v =>
            {
                masterContainer.FadeTo(v.NewValue == Visibility.Visible ? 1f : 0f, 300, Easing.OutQuint);
            });
            featureHeaderOverlay = loaded;

            Game.Add(loaded);
        });

        // Show dropdown when changing to oC ruleset
        ruleset.BindValueChanged(v =>
        {
            if (v.NewValue.ShortName != oCrsRuleset.SHORT_NAME // Current not oC
                || v.OldValue == null // No old value (start up oC)
                || v.OldValue.ShortName == oCrsRuleset.SHORT_NAME) // TriggerChange (maybe?)
            {
                return;
            }

            // Switch to osu!std
            var std = rulesets?.AvailableRulesets.FirstOrDefault();
            if (std is not null)
                rs.Value = std;
            // rs.Value = v.OldValue;

            profileButton?.reshow();
            featureHeaderOverlay?.Show();
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        colourProvider.HueColour.BindValueChanged(_ =>
        {
            background.Colour = colourProvider.Background6.Opacity(0.8f);
        }, true);
    }

    private GetUserIdFromOsuId? request;

    // Reflection hack playlists search box
    // NOTE: HACK!!! visiting private member, lazer source may change any time
    private partial class HackedPlaylistsLoungeSubScreen : PlaylistsLoungeSubScreen
    {
        private void hack()
        {
            Logging.Log("Playlists search box hack running now...");
            try
            {
                var searchTextBox = this.FindInstance(typeof(SearchTextBox)) as SearchTextBox;
                if (searchTextBox is not null)
                {
                    Logging.Log("searchTextBox is not null");
                    searchTextBox.Current.Value = "osu!Challengers";
                }
                else
                    Logging.Log("searchTextBox is null");
            }
            catch (Exception e)
            {
                Logging.LogError(e, "Hack failed! Maybe you should update the ruleset?");
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Schedule(hack);
        }
    }

    private partial class HackedPlaylists : Playlists
    {
        protected override LoungeSubScreen CreateLounge() => new HackedPlaylistsLoungeSubScreen();
    }

    private partial class MyButton : SettingsButton
    {
        public MyButton(string text)
        {
            Text = okText = text;
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;
            Width = 0.33f;
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
            Text = "You don't have an o!C profile... Play some o!C first!";
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

    private MyButton? playButton, profileButton;

    private int? lastUserId, lastChallengersId; // cache challengers profile id

    private OcStartDropdownContainer createFeatureMenu()
    {
        var overlay = new OcStartDropdownContainer
        {
        };

        playButton = new("Play latest challenge")
        {
            BackgroundColour = new Color4(243, 98, 163, 255),

            Action = () =>
            {
                Game.PerformFromScreen(s =>
                {
                    var screen = new HackedPlaylists();
                    Logging.Log("Game.PerformFromScreen, new HackedPlaylists()");
                    s.Push(screen);
                }, [typeof(MainMenu)]);
                overlay.Hide();
            }
        };
        profileButton = new("Check your o!C profile")
        {
            BackgroundColour = new Color4(243, 156, 97, 255),

            Action = () =>
            {
                void goToProfile(int challengersId)
                {
                    // Game.HandleLink($"https://osu-challenge-tracker.vercel.app/profile/{challengersId}");

                    var screenStack = Game.GetScreenStack();
                    var user = api?.LocalUser.Value;
                    Debug.Assert(user is not null);
                    screenStack?.Push(new ChallengersProfileScreen(user, challengersId));

                    overlay.Hide();
                    profileButton?.ok();
                }

                if (lastChallengersId is null)
                {
                    // goToProfile(1); // DEBUG: just return 1 for development to reduce rpc calls and speed up

                    request?.Abort();
                    request = new(api?.LocalUser?.Value?.Id ?? 0);
                    request.Finished += () => Schedule(() =>
                    {
                        lastChallengersId = request.ResponseObject;
                        if (lastChallengersId is not null)
                        {
                            goToProfile((int)lastChallengersId);
                        }
                        else
                        {
                            profileButton?.noProfile();
                        }
                    });
                    request.Failed += (_) => profileButton?.ok();

                    profileButton?.loading();
                    request.PerformAsync();
                }
                else
                    goToProfile((int)lastChallengersId);
            }
        };
        var websiteButton = new MyButton("Open o!C Nexus website")
        {
            BackgroundColour = new Color4(90, 110, 243, 255),

            Action = () =>
            {
                Game.HandleLink("https://osu-challenge-tracker.vercel.app/");
                overlay.Hide();
            }
        };

        overlay.ButtonContainer.AddRange(
        [
            playButton,
            profileButton,
            websiteButton,
        ]);

        api?.LocalUser.BindValueChanged(v =>
        {
            if (v.NewValue.Id <= 0)
            {
                playButton?.loginFirst();
                profileButton?.loginFirst();
            }
            else
            {
                playButton?.ok();
                profileButton?.ok();
                if (lastUserId != v.NewValue.Id)
                {
                    lastChallengersId = null;
                    lastUserId = v.NewValue.Id;
                }
            }
        }, runOnceImmediately: true);

        return overlay;
    }
}
