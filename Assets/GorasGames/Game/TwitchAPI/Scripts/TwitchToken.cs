using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GorasGames.Game.TwitchAPI
{
    /// <summary>
    /// Data object to parse API response for auth token into
    /// </summary>
    [Serializable]
    public class TwitchTokenResponse
    {
        public string access_token;
        public int expires_in;
        public string refresh_token;
        public string[] scope;
        public string token_type;
    }
}