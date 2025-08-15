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
            case 19637339: // Paraliyzed_evo
            case 22228239: // ThunderBirdo
            case 32657919: // fuyukiS
            case 36037557: // STORM_33
                Texture = textures.Get("Textures/badge/dev.png");
                break;

            // gfx
            case 15657407: // rteuya
            case 31708435: // JyroMenace
                Texture = textures.Get("Textures/badge/gfx.png");
                break;

            // project lead
            case 24071806: // Sourced
                Texture = textures.Get("Textures/badge/lead.png");
                break;

            // mapper
            case 4966334: // DeviousPanda
            case 12577911: // Zer0-G
                Texture = textures.Get("Textures/badge/mapper.png");
                break;

            // qa
            case 14540907: // o Guri o
            case 16863950: // Maxxor1205
            case 17274052: // TheMagicAnimals
                Texture = textures.Get("Textures/badge/qa.png");
                break;

            // artist
            case 4125185: // guageHHH5
            case 5052899: // Matrix
            case 15118952: // sugosugiii
            case 29139453: // naikou_i_guess
                Texture = textures.Get("Textures/badge/artist.png");
                break;
        }
    }
}

