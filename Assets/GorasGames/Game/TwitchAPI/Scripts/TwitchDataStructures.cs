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
    public class TwitchAPIError : TwitchAPIMessage
    {
        public string error;
    }

    [Serializable]
    public class TwitchUserData
    {
        public TwitchChannelInfos[] data;
    }

    [Serializable]
    public class TwitchChannelInfos
    {
        public string broadcaster_type;
        public string description;
        public string display_name;
        public string id;
        public string login;
        public string offline_image_url;
        public string profile_image_url;
        public string type;
        public int view_count;
        public string email;
        public string created_at;

        public override string ToString()
        {
            return "ID : " + id + " - login : " + login + " - display name : " + display_name;
        }
    }
}