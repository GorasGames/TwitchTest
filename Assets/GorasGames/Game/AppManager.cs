using GorasGames.Core.System;
using GorasGames.Game.TwitchAPI;
using System.Threading;
using UnityEngine;

namespace GorasGames.Game
{
    public class AppManager : MonoSingleton<AppManager>
    {
        TwitchChannelInfos _userDatas;
        CancellationTokenSource _cancelTokenSource;

        // Start is called before the first frame update
        async void Start()
        {
            _cancelTokenSource = new CancellationTokenSource();
            TwitchAPIAuth.Instance.InitAsync(_cancelTokenSource);

            _userDatas = await TwitchAPIRequest.Instance.GetUserDatasAsync(_cancelTokenSource);
            Debug.Log("User Datas : " + _userDatas.ToString());
            TwitchIRCManager.Instance.Connect(_userDatas);
        }

        private new void OnApplicationQuit()
        {
            if (_cancelTokenSource != null)
                _cancelTokenSource.Cancel();
            base.OnApplicationQuit();
        }
    }
}