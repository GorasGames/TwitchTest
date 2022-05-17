using GorasGames.Core.System;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GorasGames.Game.TwitchAPI
{
    /// <summary>
    /// Manage the connection to the Twitch server
    /// </summary>
    public class TwitchAPIAuth : Singleton<TwitchAPIAuth>
    {
        private TwitchAuthParameters _twitchAuthParams;

        private bool _isInit = false;
        CancellationTokenSource _cancelTokenSource;
        AsyncOperationHandle<TwitchAuthParameters> _assetLoader;
        HttpListener _httpListener;

        private string _authState;
        private TimeSpan _validationDelay = new TimeSpan(0, 1, 0);

        TwitchAccessTokenWithExpiration _accessTokenDatas;
        TwitchAccessTokenValidation _validateTokenDatas;
        TwitchAPIMessage _message;

        public bool IsAuthenticated
        {
            get { return _accessTokenDatas != null; }
        }

        #region Init

        public async void Init()
        {
            Debug.Log("[TwitchAPIAuth] Start");
            _cancelTokenSource = new CancellationTokenSource();
            _twitchAuthParams = await LoadTwitchParams();
            if (_twitchAuthParams == null)
            {
                Debug.LogError("[TwitchAPIAuth] Unable to get twitch params");
                return;
            }
            if (_isInit)
                InitTwitchAuth();
        }
        #endregion

        #region Load datas
        /// <summary>
        /// Load the Twitch Parameters asset
        /// </summary>
        /// <returns></returns>
        private async Task<TwitchAuthParameters> LoadTwitchParams()
        {
            _assetLoader = Addressables.LoadAssetAsync<TwitchAuthParameters>("TwitchAuthParams");
            _twitchAuthParams = await _assetLoader.Task;

            if (_assetLoader.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("[TwitchAPIAuth] Success retrieving Twitch auth parameters");
                _isInit = true;
                return _assetLoader.Result;
            }
            else
            {
                _isInit = false;
                return null;
            }
        }
        #endregion

        #region OAuth
        private void InitTwitchAuth()
        {

            // make sure our API helper knows our client ID (it needed for the HTTP headers)
            TwitchAPICallHelper.Instance.TwitchClientId = _twitchAuthParams.paramAppClientId;

            _authState = ((Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
            Debug.Log("[TwitchAPIAuth] Auth state : " + _authState);
            StartLocalWebserver();
            string scopes = String.Join("+", _twitchAuthParams.scopes);
            Debug.Log("Scopes : " + scopes);
            string formatedAuthUrl = _twitchAuthParams.authorizeRequest.endPoint + String.Format(_twitchAuthParams.authorizeRequest.paramsFormat, _twitchAuthParams.paramAppClientId, _twitchAuthParams.paramRedirectUri, scopes, _authState);
            Debug.Log("Url opened = " + formatedAuthUrl);
            Application.OpenURL(formatedAuthUrl);
        }

        /// <summary>
        /// Opens a simple "webserver" like thing on localhost:8080 for the auth redirect to land on.
        /// Based on the C# HttpListener docs: https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener
        /// </summary>
        private void StartLocalWebserver()
        {
            _httpListener = new HttpListener();

            _httpListener.Prefixes.Add(_twitchAuthParams.paramRedirectUri);
            _httpListener.Start();
            _httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), _httpListener);
        }
        /// <summary>
        /// Handles the incoming HTTP request
        /// </summary>
        /// <param name="result"></param>
        private void ListenerCallback(IAsyncResult result)
        {
            Debug.Log("[TwitchAPIAuth] Incoming HTTP Request");
            HttpListenerContext httpContext;
            HttpListenerRequest httpRequest;
            string responseString;

            // fetch the context object
            httpContext = _httpListener.EndGetContext(result);

            // the context object has the request object for us, that holds details about the incoming request
            httpRequest = httpContext.Request;

            string state = httpRequest.QueryString.Get("state");
            string code = httpRequest.QueryString.Get("code");

            // check that we got a code value and the state value matches our remembered one
            if (state == _authState)
            {
                // if all checks out, use the code to exchange it for the actual auth token at the API
                Debug.Log("[TwitchAPIAuth] State valid");
                GetTokenFromCode(code);
            }

            // Send web response to the user
            // TODO : close the web page
            HttpListenerResponse httpResponse;
            // build a response to send an "ok" back to the browser for the user to see
            httpResponse = httpContext.Response;
            responseString = "<html><body><b>DONE!</b><br>(You can close this tab/window now)</body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // send the output to the client browser
            httpResponse.ContentLength64 = buffer.Length;
            Stream output = httpResponse.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            // the HTTP listener has served it's purpose, shut it down
            _httpListener.Stop();
        }

        /// <summary>
        /// Makes the API call to exchange the received code for the actual auth token
        /// </summary>
        /// <param name="code">The code parameter received in the callback HTTP reuqest</param>
        private async void GetTokenFromCode(string code)
        {
            Debug.Log("Get token from code");
            string postUrl = _twitchAuthParams.getTokenRequest.endPoint + String.Format(_twitchAuthParams.getTokenRequest.paramsFormat, _twitchAuthParams.paramAppClientId, _twitchAuthParams.paramAppClientSecret, code, _twitchAuthParams.paramAuthorizationCode, _twitchAuthParams.paramRedirectUri);
            Debug.Log("Url for POST request : " + postUrl);

            // make the call!
            string apiResponseJson = await TwitchAPICallHelper.Instance.CallApiAsync(postUrl, "POST");

            Debug.Log("Access token datas : " + apiResponseJson);
            // parse the return JSON into a more usable data object
            _accessTokenDatas = JsonUtility.FromJson<TwitchAccessTokenWithExpiration>(apiResponseJson);
            TwitchAPICallHelper.Instance.TwitchAuthToken = _accessTokenDatas.access_token;

            PlayerPrefs.SetString("AccessToken", _accessTokenDatas.access_token);
            PlayerPrefs.SetString("RefreshToken", _accessTokenDatas.refresh_token);
            PlayerPrefs.Save();

            ValidateToken();
        }

        private async void RefreshToken()
        {
            Debug.Log("[TwitchAPIAuth] Refresh token");
            string postBody = String.Format(_twitchAuthParams.refreshTokenRequest.paramsFormat, _twitchAuthParams.paramAppClientId, _twitchAuthParams.paramAppClientSecret, UnityWebRequest.EscapeURL(PlayerPrefs.GetString("RefreshToken")));
            Debug.Log("Post Url : " + postBody);
            string response = await TwitchAPICallHelper.Instance.CallApiAsync(_twitchAuthParams.refreshTokenRequest.endPoint, "POST", postBody);
            Debug.Log("Response : " + response);

            TwitchAccessToken _refreshToken = JsonUtility.FromJson<TwitchAccessToken>(response);

            TwitchAPICallHelper.Instance.TwitchAuthToken = _refreshToken.access_token;

            PlayerPrefs.SetString("AccessToken", _refreshToken.access_token);
            PlayerPrefs.SetString("RefreshToken", _refreshToken.refresh_token);
        }

        private async void ValidateToken()
        {
            string response;

            do
            {
                Debug.Log("[TwitchAPIAuth] Validate token");
                response = await TwitchAPICallHelper.Instance.CallApiAsync(_twitchAuthParams.validateTokenRequest.endPoint);

                if (response.Contains("status"))
                {
                    _message = JsonUtility.FromJson<TwitchAPIMessage>(response);
                    Debug.LogError("[TwitchAPIAuth] Error " + _message.status + " : Unable to validate token. Reason : " + _message.message);

                }
                else
                {
                    _validateTokenDatas = JsonUtility.FromJson<TwitchAccessTokenValidation>(response);
                    Debug.Log("Validate token expires in : " + _validateTokenDatas.expires_in);
                }

                await Task.Delay(_validationDelay, _cancelTokenSource.Token);
            } while (_isInit);
        }
        #endregion

        private void Terminate()
        {
            _cancelTokenSource.Cancel();
            Addressables.Release(_assetLoader);
            _isInit = false;
        }
    }
}