using GorasGames.Core.System;
using GorasGames.Game.TwitchAPI;

namespace GorasGames.Game
{
    public class AppManager : MonoSingleton<AppManager>
    {
        // Start is called before the first frame update
        void Start()
        {
            TwitchAPIAuth.Instance.Init();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}