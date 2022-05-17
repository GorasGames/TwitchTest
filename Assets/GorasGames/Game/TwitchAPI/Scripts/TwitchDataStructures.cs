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
    public class TwitchAccessToken
    {
        public string access_token;
        public string refresh_token;
        public string[] scope;
        public string token_type;
    }

    [Serializable]
    public class TwitchAccessTokenWithExpiration : TwitchAccessToken
    {
        public int expires_in;
    }

    [Serializable]
    public class TwitchAccessTokenValidation
    {
        public string client_id;
        public string login;
        public string[] scopes;
        public string user_id;
        public string expires_in;
    }

    [Serializable]
    public class TwitchAPIMessage
    {
        public string status;
        public string message;
    }

    [Serializable]
    public class TwitchAPIError: TwitchAPIMessage
    {
        public string error;
    }
}