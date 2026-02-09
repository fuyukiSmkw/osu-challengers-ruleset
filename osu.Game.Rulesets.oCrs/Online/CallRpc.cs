// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System.Net.Http;

namespace osu.Game.Rulesets.oCrs.Online;

public class CallRpc<T> : JsonWebRequestExtended<T>
{
    protected const string SUPABASE_URL = "https://msyttvgjzsegrdciplck.supabase.co";
    protected const string SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im1zeXR0dmdqenNlZ3JkY2lwbGNrIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njk1MjQyNTksImV4cCI6MjA4NTEwMDI1OX0.sRyQoirmz6JtWNxE62vjjXaBzzLOP6sP1xjUzputSpk";

    public CallRpc(string rpcName, params object[] args)
        : base($"{SUPABASE_URL}/rest/v1/rpc/{rpcName}", args)
    {
        Method = HttpMethod.Post;
        AddHeader("apikey", SUPABASE_ANON_KEY);
        AddHeader("Authorization", $"Bearer {SUPABASE_ANON_KEY}");
        // ContentType = "application/json";
        // AddHeader("Accept", "application/json");
    }
}
