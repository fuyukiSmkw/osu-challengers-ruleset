// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Replays;
using osuTK;

namespace osu.Game.Rulesets.oCrs.Replays
{
    public class oCrsReplayFrame : ReplayFrame
    {
        public List<oCrsAction> Actions = new List<oCrsAction>();
        public Vector2 Position;

        public oCrsReplayFrame(oCrsAction? button = null)
        {
            if (button.HasValue)
                Actions.Add(button.Value);
        }

        public override bool IsEquivalentTo(ReplayFrame other)
            => other is oCrsReplayFrame freeformFrame && Time == freeformFrame.Time && Position == freeformFrame.Position && Actions.SequenceEqual(freeformFrame.Actions);
    }
}
