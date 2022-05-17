using GorasGames.Core.System;

namespace GorasGames.Game.TwitchAPI
{
    /// <summary>
    /// Tools provided to send many requests about Twitch channel when the streamer is connected
    /// </summary>
    public class TwitchAPITools : Singleton<TwitchAPITools>
    {

        public void GetChannelInfos()
        {
            if(TwitchAPIAuth.Instance.IsAuthenticated)
            {

            }
        }
    }
}