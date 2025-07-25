// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System.Threading.Tasks;
using osu.Framework.IO.Network;

namespace osu.Game.Rulesets.oCrs.Online;

public partial class JsonWebRequestExtended<T>(string url = null, params object[] args) : JsonWebRequest<T>(url, args)
{
    public Task<T> AwaitRequest()
    {
        var tcs = new TaskCompletionSource<T>();

        Finished += () => tcs.TrySetResult(ResponseObject);
        Failed += ex => tcs.TrySetException(ex);

        PerformAsync();

        return tcs.Task;
    }
}

