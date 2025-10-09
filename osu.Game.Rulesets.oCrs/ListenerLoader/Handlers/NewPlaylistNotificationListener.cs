// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osu.Game.Beatmaps.Drawables.Cards;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Rulesets.oCrs.Configuration;
using osu.Game.Rulesets.oCrs.Graphics;
using osu.Game.Rulesets.oCrs.ListenerLoader.Utils;
using osu.Game.Screens.Menu;
using osu.Game.Screens.OnlinePlay.Lounge.Components;
using osuTK;
using static osu.Game.Rulesets.oCrs.ListenerLoader.Handlers.OcStartDropdownListener;

#nullable enable

namespace osu.Game.Rulesets.oCrs.ListenerLoader.Handlers;

public partial class NewPlaylistNotificationListener : AbstractHandler
{
    private static readonly int[] host_uid = [24071806];
    private const int max_notification_count = 3;
    private readonly double time_between_retries = 5000.0; // ms
    private readonly int max_retry_count = 5;

    #region notifications

    // weekly: only keep the word after "Best" (mod challenge)
    [GeneratedRegex(@"Best\s+(\w+)(?:.*)", RegexOptions.IgnoreCase, "")]
    private static partial Regex RegexWeekly();
    private const string template_weekly = "osu!Challengers Weekly now live!\nMod Challenge: {0}";

    // CE
    [GeneratedRegex(@"osu!Challengers CE")]
    private static partial Regex RegexCE();
    private const string template_ce = "osu!Challengers CE now live!";

    protected static string getNotificationText(RoomExtended room)
    {

        Match match = RegexWeekly().Match(room.Name);
        if (match.Success && match.Groups.Count > 1)
        {
            return string.Format(template_weekly, match.Groups[1].Value);
        }

        // CE: search for CE
        match = RegexCE().Match(room.Name);
        if (match.Success)
        {
            return string.Format(template_ce);
        }

        // fallback
        string templateFallback = "osu!Challengers Playlist now live: \"{0}\"";
        return string.Format(templateFallback, room.Name);
    }

    public partial class SingleItemChallengersNotification : SimpleNotification
    {
        private readonly RoomExtended room;
        protected OcIcon ocIcon { get; private set; } = null!;
        private BeatmapCardNano card = null!;

        public SingleItemChallengersNotification(RoomExtended room)
        {
            Debug.Assert(room.Playlist.Count == 1);
            this.room = room;
            Text = getNotificationText(room);
            Icon = default;
        }

        [BackgroundDependencyLoader]
        private void load(OsuGame? game)
        {
            Content.Add(card = new BeatmapCardNano((APIBeatmapSet)room.Playlist.Single().Beatmap.BeatmapSet!));
            Activated = () =>
            {
                game?.PerformFromScreen(mainMenuScreen =>
                {
                    var loungeScreen = new HackedPlaylists();
                    mainMenuScreen.Push(loungeScreen);
                    loungeScreen.Join(room);
                }, [typeof(MainMenu)]);
                return true;
            };

            IconContent.Masking = true;
            IconContent.CornerRadius = CORNER_RADIUS;
            IconContent.ChangeChildDepth(IconDrawable, float.MinValue);
            LoadComponentAsync(ocIcon = new OcIcon()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            }, IconContent.Add);
        }

        protected override void Update()
        {
            base.Update();
            card.Width = Content.DrawWidth;
        }
    }

    public partial class MultipleItemChallengersRoomNotification : SimpleNotification
    {
        private readonly RoomExtended room;
        protected OcIcon ocIcon { get; private set; } = null!;

        public MultipleItemChallengersRoomNotification(RoomExtended room)
        {
            this.room = room;
            Text = getNotificationText(room);
            Icon = default;
        }

        [BackgroundDependencyLoader]
        private void load(OsuGame? game)
        {
            IconContent.Masking = true;
            IconContent.CornerRadius = CORNER_RADIUS;
            IconContent.ChangeChildDepth(IconDrawable, float.MinValue);

            Activated = () =>
            {
                game?.PerformFromScreen(mainMenuScreen =>
                {
                    var loungeScreen = new HackedPlaylists();
                    mainMenuScreen.Push(loungeScreen);
                    loungeScreen.Join(room);
                }, [typeof(MainMenu)]);
                return true;
            };

            LoadComponentAsync(ocIcon = new OcIcon()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(1.5f),
            }, IconContent.Add);
        }

        protected override void Update()
        {
            base.Update();
            IconContent.Width = IconContent.DrawHeight;
        }
    }

    #endregion notifications

    [Resolved]
    private IAPIProvider api { get; set; } = null!;

    [Resolved]
    private INotificationOverlay? notificationOverlay { get; set; } = null!;

    [Resolved]
    private RulesetStore rulesets { get; set; } = null!;

    #region getLatestChallengersRoom

    private bool hasResultRooms = false;
    private GetRoomsRequest lastReq = null!;
    private double? lastTimeReq;
    private Task<bool> requestRooms()
    {
        lastReq?.Cancel();
        var tcs = new TaskCompletionSource<bool>();
        var req = new GetRoomsRequest(createFilterCriteria());

        req.Success += result =>
        {
            roomsReceived(result.Where(r => host_uid.Contains(r?.Host?.Id ?? 0)).ToArray());
            tcs.SetResult(true);
        };
        req.Failure += _ => tcs.SetResult(false);

        api.Queue(req);
        lastReq = req;
        return tcs.Task;
    }
    private void roomsReceived(Room[] rooms)
    {
        hasResultRooms = true;
        HashSet<string> roomIds = [];
        foreach (var room in rooms)
        {
            fetchRoom(room);
            roomIds.Add(room.RoomID.ToString()!);
        }
        // clear old, not active rooms in notification counts json
        var obj = JsonConvert.DeserializeObject<Dictionary<string, int>>(notificationCountsJson.Value)!;
        var keysToRemove = obj.Keys.Except(roomIds).ToList();
        foreach (string key in keysToRemove)
            obj.Remove(key);
        notificationCountsJson.Value = JsonConvert.SerializeObject(obj);
    }

    public struct CurrentUserScoreObject
    {
        public CurrentUserScoreObject() { this = default; }
        [JsonProperty("accuracy")] public double accuracy = 0;
        [JsonProperty("attempts")] public long attempts = 0;
        [JsonProperty("completed")] public long completed = 0;
        [JsonProperty("pp")] public double pp = 0;
        [JsonProperty("room_id")] public long roomId = 0;
        [JsonProperty("total_score")] public long totalScore = 0;
        [JsonProperty("user_id")] public long userId = 0;
    }
    public class RoomExtended : Room
    {
        [JsonProperty("current_user_score")]
        public CurrentUserScoreObject currentUserScore = default;
    }
    private class GetRoomExtendedRequest(long roomId) : APIRequest<RoomExtended>
    {
        public readonly long RoomId = roomId;

        protected override string Target => $"rooms/{RoomId}";
    }

    private void fetchRoom(Room room, int tryCount = 1)
    {
        long id = room.RoomID ?? 0;
        if (tryCount > max_retry_count)
        {
            Logging.Log($"Tried and failed {max_retry_count} times to fetchRoom room ID {id}, give up.", level: Framework.Logging.LogLevel.Important);
            return;
        }

        var req = new GetRoomExtendedRequest(id);
        req.Success += sendNotification;
        req.Failure += _ => Scheduler.AddDelayed(() => fetchRoom(room, tryCount + 1), time_between_retries);
        api.Queue(req);
    }

    private ScheduledDelegate? scheduledReq;
    private bool reqActive = false;
    private int reqCount = 0;
    private void doReq()
    {
        Debug.Assert(ThreadSafety.IsUpdateThread);
        scheduledReq = null;
        reqActive = true;
        if (++reqCount > max_retry_count)
        {
            Logging.Log($"Tried and failed {max_retry_count} times to fetch room list, give up.", level: Framework.Logging.LogLevel.Important);
            return;
        }
        requestRooms().ContinueWith(_ => reqComplete());
    }

    private void reqComplete()
    {
        lastTimeReq = Time.Current;
        reqActive = false;
        if (scheduledReq == null)
            reqIfNecessary();
    }

    private void reqIfNecessary()
    {
        if (!IsLoaded) return;
        if (reqActive) return;
        if (hasResultRooms) return; // dont have to req anymore
        if (time_between_retries == 0) return;
        if (!lastTimeReq.HasValue)
        {
            Scheduler.AddOnce(doReq);
            return;
        }
        if (Time.Current - lastTimeReq.Value > time_between_retries)
        {
            Scheduler.AddOnce(doReq);
            return;
        }
        // not enough time
        scheduleNextReq();
    }
    private void scheduleNextReq()
    {
        scheduledReq?.Cancel();
        double lastReqDuration = lastTimeReq.HasValue ? Time.Current - lastTimeReq.Value : 0;
        scheduledReq = Scheduler.AddDelayed(doReq, Math.Max(0, time_between_retries - lastReqDuration));
    }


    private FilterCriteria createFilterCriteria() => new()
    {
        SearchString = "osu!Challengers",
        Ruleset = rulesets?.AvailableRulesets.FirstOrDefault(), // std
        Mode = RoomModeFilter.Open,
        Category = @"normal"
    };

    #endregion getLatestChallengersRoom

    #region sendNotification

    private void sendNotification(RoomExtended room)
    {
        if (!alwaysShowNotifications.Value)
        {
            // ignore if user has played this room
            if (room.currentUserScore.attempts > 0)
            {
                Logging.Log($"current user has played the room {room.RoomID}/{room.Name}, ignore this.");
                return;
            }
            // ignore if the room has been shown for at least max_notification_count times
            var obj = JsonConvert.DeserializeObject<Dictionary<string, int>>(notificationCountsJson.Value)!;
            string key = room.RoomID.ToString()!;
            if (obj.TryGetValue(key, out int count) && count >= max_notification_count)
            {
                Logging.Log($"room {room.RoomID}/{room.Name} has shown notification for {count} times, ignore this.");
                return;
            }
            obj[key] = count + 1;
            notificationCountsJson.Value = JsonConvert.SerializeObject(obj);
        }
        if (room.Playlist.Count == 1)
            notificationOverlay?.Post(new SingleItemChallengersNotification(room));
        else
            notificationOverlay?.Post(new MultipleItemChallengersRoomNotification(room));
    }

    #endregion sendNotification

    private readonly Bindable<bool> enableNotifications = new(true);
    private readonly Bindable<bool> alwaysShowNotifications = new(false);
    private readonly Bindable<string> notificationCountsJson = new("{}");
    private oCrsRulesetConfigManager? oCrsRulesetConfig = null;
    private readonly double getSettingsDelay = 1000; // ms

    private void getSettingsAndStart()
    {
        oCrsRulesetConfig = (oCrsRulesetConfigManager?)(Game.Dependencies as DependencyContainer)!.getFromCache<oCrsRulesetConfigManager>();
        if (oCrsRulesetConfig is null) // should not happen though
        {
            Logging.Log($"settings is null, retry after {getSettingsDelay}");
            Scheduler.AddDelayed(getSettingsAndStart, getSettingsDelay);
            return;
        }
        oCrsRulesetConfig.BindWith(oCrsRulesetSettings.EnableNewChallengersPlaylistNotification, enableNotifications);
        oCrsRulesetConfig.BindWith(oCrsRulesetSettings.EnableAlwaysShowNewChallengersNotification, alwaysShowNotifications);
        oCrsRulesetConfig.BindWith(oCrsRulesetSettings.ChallengersNotificationCountsJson, notificationCountsJson);
        enableNotifications.BindValueChanged(v =>
        {
            if (v.NewValue)
                reqIfNecessary();
            else
            {
                // if user re-enable within this session, show notification
                hasResultRooms = false;
                // clear notification counts
                notificationCountsJson.Value = "{}";
            }
        }, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        getSettingsAndStart();
    }
}
