// Copyright (c) 2025 fuyukiS <fuyukiS@outlook.jp>. Licensed under the MIT License.
// See the LICENCE file in the repository root for full licence text.

using System.Net.Http;

namespace osu.Game.Rulesets.oCrs.Online;

public class CallRpc<T> : JsonWebRequestExtended<T>
{
    protected const string SUPABASE_URL = "https://yqgqoxgykswytoswqpkj.supabase.co";
    protected const string SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InlxZ3FveGd5a3N3eXRvc3dxcGtqIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDg3MTkxNTEsImV4cCI6MjA2NDI5NTE1MX0.cIWfvz9dlSWwYy7QKSmWpEHc1KVzpB77VzB7TNhQ2ec";

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
