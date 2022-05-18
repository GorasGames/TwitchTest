using GorasGames.Core.System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GorasGames.Game.TwitchAPI
{
    public class TwitchAPIRequest : Singleton<TwitchAPIRequest>
    {
        string getUserInfos = "https://api.twitch.tv/helix/users?";

        /// <summary>
        /// Get the streamer User Infos
        /// </summary>
        /// <param name="pCancellationToken"></param>
        /// <returns></returns>
        public async Task<TwitchChannelInfos> GetUserDatasAsync(CancellationTokenSource pCancellationToken)
        {
            // Waiting for auth token available
            while(string.IsNullOrEmpty(TwitchAPICallHelper.Instance.TwitchAuthToken))
            {
                await Task.Delay(100, pCancellationToken.Token);
            }
            // Call API to get user infos
            string result = await TwitchAPICallHelper.Instance.CallApiAsync(getUserInfos);
            // Deserialize user infos
            TwitchUserData userData = JsonUtility.FromJson<TwitchUserData>(result);
            
            return userData.data[0];
        }
    }
}