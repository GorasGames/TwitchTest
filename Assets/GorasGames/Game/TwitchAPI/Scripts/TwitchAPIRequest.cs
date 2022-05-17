using GorasGames.Core.System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GorasGames.Game.TwitchAPI
{
    public class TwitchAPIRequest : Singleton<TwitchAPIRequest>
    {
        string getUserInfos = "https://api.twitch.tv/helix/users?";

        public async Task<TwitchUserDatas> GetUserDatasAsync(CancellationTokenSource pCancellationToken)
        {
            while(string.IsNullOrEmpty(TwitchAPICallHelper.Instance.TwitchAuthToken))
            {
                Debug.Log("[TwitchAPIRequest] Waiting for auth...");
                await Task.Delay(100, pCancellationToken.Token);
            }

            string result = await TwitchAPICallHelper.Instance.CallApiAsync(getUserInfos);

            Debug.Log("[TwitchAPIRequest] Get User Infos : " + result);
            return JsonUtility.FromJson<TwitchUserDatas>(result);
        }
    }
}