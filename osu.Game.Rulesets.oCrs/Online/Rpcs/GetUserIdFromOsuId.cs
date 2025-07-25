// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.oCrs.Online.Rpcs;

public class GetUserIdFromOsuId : CallRpc<int?>
{
    public GetUserIdFromOsuId(int osuId)
    : base("get_user_id_from_osu_id")
    {
        AddRaw("{\"p_osu_id\":" + osuId.ToString() + "}");
        Failed += (e) =>
        {
            Logging.LogError(e, $"Error requesting GetUserIdFromOsuId, osuId is {osuId}");
        };
    }
}
