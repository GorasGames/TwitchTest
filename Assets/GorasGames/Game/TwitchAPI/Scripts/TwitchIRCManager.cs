using GorasGames.Core.System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace GorasGames.Game.TwitchAPI
{
    public class TwitchIRCManager : MonoSingleton<TwitchIRCManager>
    {
        TcpClient TwitchClient;
        StreamReader _reader;
        StreamWriter _writer;

        const string URL = "irc.chat.twitch.tv";
        const int PORT = 6667;

        string _currentMsg;

        bool _isWaitingForPong = false;
        bool _connected = false;
        string _pingMessage;

        public void Connect(TwitchChannelInfos pUser)
        {
            Debug.Log("[TwitchIRCManager] Try to connect");
            TwitchClient = new TcpClient(URL, PORT);
            _reader = new StreamReader(TwitchClient.GetStream());
            _writer = new StreamWriter(TwitchClient.GetStream());

            _writer.WriteLine("PASS oauth:" + TwitchAPICallHelper.Instance.TwitchAuthToken);
            _writer.WriteLine("NICK " + pUser.login.ToLower());
            _writer.WriteLine("JOIN #" + pUser.display_name.ToLower());
            _writer.Flush();

            _connected = true;
        }

        private void Update()
        {
            if (_connected)
            {
                if (TwitchClient.Available > 0)
                {
                    _currentMsg = _reader.ReadLine();

                    if(_currentMsg.StartsWith("PING"))
                    {
                        _isWaitingForPong = true;
                        _pingMessage = _currentMsg;
                    }

                    Debug.Log("Message : " + _currentMsg);
                }
            }

            if(!string.IsNullOrEmpty(_pingMessage))
            {
                _writer = new StreamWriter(TwitchClient.GetStream());
                _writer.WriteLine("PONG " + _pingMessage);
                _writer.Flush();

                _pingMessage = string.Empty;
            }
        }
    }
}