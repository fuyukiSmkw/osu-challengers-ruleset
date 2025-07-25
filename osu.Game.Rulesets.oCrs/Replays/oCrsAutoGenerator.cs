// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.oCrs.Objects;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.oCrs.Replays
{
    public class oCrsAutoGenerator : AutoGenerator<oCrsReplayFrame>
    {
        public new Beatmap<oCrsHitObject> Beatmap => (Beatmap<oCrsHitObject>)base.Beatmap;

        public oCrsAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override void GenerateFrames()
        {
            Frames.Add(new oCrsReplayFrame());

            foreach (oCrsHitObject hitObject in Beatmap.HitObjects)
            {
                Frames.Add(new oCrsReplayFrame
                {
                    Time = hitObject.StartTime,
                    Position = hitObject.Position,
                    // todo: add required inputs and extra frames.
                });
            }
        }
    }
}
