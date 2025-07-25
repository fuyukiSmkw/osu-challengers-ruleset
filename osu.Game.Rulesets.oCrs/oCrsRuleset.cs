﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// Copyright (c) 2025 MATRIX-feather. Licensed under the MIT Licence.
// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.oCrs.Beatmaps;
using osu.Game.Rulesets.oCrs.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;
using osu.Framework.Allocation;
using osu.Game.Database;
using osu.Game.Online.API;
using osu.Framework.Platform;
using osu.Framework.Logging;

namespace osu.Game.Rulesets.oCrs
{
    public partial class oCrsRuleset : Ruleset
    {
        public override string Description => "osu!Challengers";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) =>
            new DrawableoCrsRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) =>
            new oCrsBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) =>
            new oCrsDifficultyCalculator(RulesetInfo, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                default:
                    return [];
            }
        }

        public static readonly string SHORT_NAME = "ocrsruleset";
        public override string ShortName => SHORT_NAME;

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => [];

        public override Drawable CreateIcon() => new OCIcon();

        public partial class OCIcon : CompositeDrawable
        {
            public OCIcon()
            {
                AutoSizeAxes = Axes.Both;
                InternalChildren =
                [
                    new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Scale = new Vector2(20f),
                        Icon = FontAwesome.Regular.Circle,
                        Colour = Color4.White,
                    },
                    new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Y = -0.5f,
                        Text = "o!C",
                        Font = OsuFont.Torus.With(size: 10, weight: FontWeight.SemiBold),
                        Colour = Color4.White,
                    }
                ];
            }

            [BackgroundDependencyLoader(permitNulls: true)]
            private void load(OsuGame game, Storage storage, IModelImporter<BeatmapSetInfo> beatmapImporter, IAPIProvider api)
            {
                try
                {
                    Logging.Log("Begin init ListenerLoader");
                    Logging.Log($"Deps: Game = '{game}' :: Storage = '{storage}' :: Importer = '{beatmapImporter}' :: IAPIProvider = '{api}'");

                    if (ListenerLoader.ListenerLoader.INSTANCE.BeginInject(storage, game, Scheduler))
                        return;

                    Logging.Log("Injection failed!", level: LogLevel.Error);
                    return;
                }
                catch (Exception e)
                {
                    Logging.LogError(e, "Unknown exception");
                }
            }
        }

        // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;
    }
}
