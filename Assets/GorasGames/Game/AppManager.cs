using GorasGames.Core.System;
using GorasGames.Game.TwitchAPI;
using System.Threading;

namespace GorasGames.Game
{
    public class AppManager : MonoSingleton<AppManager>
    {
        TwitchUserDatas _userDatas;
        CancellationTokenSource _cancelTokenSource;

        // Start is called before the first frame update
        async void Start()
        {
            _cancelTokenSource = new CancellationTokenSource();
            TwitchAPIAuth.Instance.InitAsync(_cancelTokenSource);

            _userDatas = await TwitchAPIRequest.Instance.GetUserDatasAsync(_cancelTokenSource);
        }

        private new void OnApplicationQuit()
        {
            if (_cancelTokenSource != null)
                _cancelTokenSource.Cancel();
            base.OnApplicationQuit();
        }
    }
}