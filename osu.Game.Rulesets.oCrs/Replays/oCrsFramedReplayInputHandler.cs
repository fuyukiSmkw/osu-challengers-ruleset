﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Input.StateChanges;
using osu.Framework.Utils;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.oCrs.Replays
{
    public class oCrsFramedReplayInputHandler : FramedReplayInputHandler<oCrsReplayFrame>
    {
        public oCrsFramedReplayInputHandler(Replay replay)
            : base(replay)
        {
        }

        protected override bool IsImportant(oCrsReplayFrame frame) => frame.Actions.Any();

        protected override void CollectReplayInputs(List<IInput> inputs)
        {
            var position = Interpolation.ValueAt(CurrentTime, StartFrame.Position, EndFrame.Position, StartFrame.Time, EndFrame.Time);

            inputs.Add(new MousePositionAbsoluteInput
            {
                Position = GamefieldToScreenSpace(position),
            });
            inputs.Add(new ReplayState<oCrsAction>
            {
                PressedActions = CurrentFrame?.Actions ?? new List<oCrsAction>(),
            });
        }
    }
}
