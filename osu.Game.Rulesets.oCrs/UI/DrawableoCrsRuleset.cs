// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.oCrs.Objects;
using osu.Game.Rulesets.oCrs.Objects.Drawables;
using osu.Game.Rulesets.oCrs.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.oCrs.UI
{
    [Cached]
    public partial class DrawableoCrsRuleset : DrawableRuleset<oCrsHitObject>
    {
        public DrawableoCrsRuleset(oCrsRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override Playfield CreatePlayfield() => new oCrsPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new oCrsFramedReplayInputHandler(replay);

        public override DrawableHitObject<oCrsHitObject> CreateDrawableRepresentation(oCrsHitObject h) => new DrawableoCrsHitObject(h);

        protected override PassThroughInputManager CreateInputManager() => new oCrsInputManager(Ruleset?.RulesetInfo);
    }
}
