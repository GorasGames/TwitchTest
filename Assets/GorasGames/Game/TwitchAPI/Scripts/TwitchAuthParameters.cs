using System;
using UnityEngine;

namespace GorasGames.Game.TwitchAPI
{
    [CreateAssetMenu(fileName = "TwitchAuthParameters", menuName ="Twitch/Parameters", order =1)]
    public class TwitchAuthParameters : ScriptableObject
    {
        [Header("API Get Code")]
        public TwitchRequestData authorizeRequest;
        [Header("API Access Token")]
        public TwitchRequestData getTokenRequest;
        [Header("API Validate Token")]
        public TwitchRequestData validateTokenRequest;
        [Header("API Refresh Token")]
        public TwitchRequestData refreshTokenRequest;

        [Header("Parameters")]
        public string paramAppClientId;
        public string paramAppClientSecret;
        public string paramRedirectUri;
        public string paramGetTokenGrantType;

        public string[] scopes = new string[]
        {
            "user:read:email",
            "chat:edit",
            "chat:read",
            "channel:read:redemptions",
            "channel_subscriptions",
            "user:read:broadcast",
            "user:edit:broadcast",
            "channel:manage:redemptions"
        };
    }

    [Serializable]
    public class TwitchRequestData
    {
        public string endPoint;
        public string paramsFormat;
    }
}