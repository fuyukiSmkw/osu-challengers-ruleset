// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Rulesets.oCrs.Screens.ChallengersProfile.Components;

public partial class ChallengersBadge : Sprite
{
    private APIUser user;

    public ChallengersBadge(APIUser user)
    {
        this.user = user;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        textures.AddTextureSource(new TextureLoaderStore(new oCrsRuleset().CreateResourceStore()));
        switch (user.Id)
        {
            // dev
            case 19637339:
            case 22228239:
            case 32657919:
                Texture = textures.Get("Textures/badge/dev.png");
                break;

            // gfx
            case 15657407:
                Texture = textures.Get("Textures/badge/gfx.png");
                break;

            // project lead
            case 24071806:
                Texture = textures.Get("Textures/badge/lead.png");
                break;
        }
    }
}
