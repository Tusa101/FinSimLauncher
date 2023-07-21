using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic;
using System.Diagnostics;
using FinSimLauncher.Login;

namespace FinSimLauncher.LoginForm.ViewModel
{
    public class GoogleAuthVM
    {
        
        // client configuration
        private const string _clientID = "770522826914-ckgl9nkpptgkk8fjdmqtsu73jcbiq3f8.apps.googleusercontent.com";
        private const string _clientSecret = "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";
        private const string _authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string _tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        private const string _userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";
        private string _currSubID = "";

        public string SubID { get => _currSubID; set => _currSubID = value; }

        public GoogleAuthVM()
        {
        }

        public async Task<bool> MainAuthMethod(Window window)
        {
            // Generates state and PKCE values.
            string state = RandomDataBase64URL(32);
            string code_verifier = RandomDataBase64URL(32);
            string code_challenge = Base64URLEncodeNoPadding(SHA256(code_verifier));
            const string code_challenge_method = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            //output("redirect URI: " + redirectURI);

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);
            ////output("Listening..");
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                _authorizationEndpoint,
                System.Uri.EscapeDataString(redirectURI),
                _clientID,
                state,
                code_challenge,
                code_challenge_method);

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings this app back to the foreground.
            window.Activate();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            var buffer = System.Text.Encoding.UTF8.GetBytes("<html><head><meta http-equiv='refresh' content='10;url=http://di-finsim.tilda.ws'><style type=\"text/css\">.myCard{background-color:transparent;width:190px;height:254px;perspective:1000px;position:fixed;top:50%;left:50%;-webkit-transform:translate(-50%,-50%);transform:translate(-50%,-50%);}.title{font-size:1.5em;font-weight:900;text-align:center;margin:0;}.innerCard{position:relative;width:100%;height:100%;text-align:center;transition:transform 0.8s;transform-style:preserve-3d;cursor:pointer;}.myCard:hover .innerCard{transform:rotateY(180deg);}.frontSide,.backSide{position:absolute;display:flex;flex-direction:column;align-items:center;justify-content:space-evenly;width:100%;height:100%;-webkit-backface-visibility:hidden;backface-visibility:hidden;border:1px solid rgba(255,255,255,0.8);border-radius:1rem;color:white;box-shadow:0 0 0.3em rgba(255,255,255,0.5);font-weight:700;}.frontSide,.frontSide::before{background:linear-gradient(43deg,rgb(65,88,208)0%,rgb(200,80,192)46%,rgb(255,204,112)100%);}.backSide,.backSide::before{background-image:linear-gradient(160deg,#0093E9 0%,#80D0C7 100%);}.backSide{transform:rotateY(180deg);}.frontSide::before,.backSide::before{top:50%;left:50%;transform:translate(-50%,-50%);content:'';width:110%;height:110%;position:absolute;z-index:-1;border-radius:1em;filter:blur(20px);animation:animate 5s linear infinite;}@keyframes animate{0%{opacity: 0.3;}80%{opacity: 1;}100%{opacity: 0.3;}}</style></head><body><div class=\"myCard\"><div class=\"innerCard\"><div class=\"frontSide\"><p class=\"title\">Successfully logged in!</p><p>Get back to the FinSim Launcher</p></div><div class=\"backSide\"><p class=\"title\">Successfully logged in!</p><p>Get back to the FinSim Launcher</p></div></div></div></body></html>");
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                //output(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return false;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                //output("Malformed authorization response. " + context.Request.QueryString);
                return false;
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incoming_state = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incoming_state != state)
            {
                //output(String.Format("Received request with invalid state ({0})", incoming_state));
                return false;
            }
            //output("Authorization code: " + code);

            // Starts the code exchange at the Token Endpoint.

            await Task.WhenAll(new Task[] { PerformCodeExchange(code, code_verifier, redirectURI) });
            return true;
        }
        
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        async Task PerformCodeExchange(string code, string code_verifier, string redirectURI)
        {
            //output("Exchanging code for tokens...");

            // builds the  request
            string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(redirectURI),
                _clientID,
                code_verifier,
                _clientSecret
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();
                    //output(responseText);

                    // converts to dictionary
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    string access_token = tokenEndpointDecoded["access_token"];
                    UserInfoCall(access_token);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        //output("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            //output(responseText);
                        }
                    }

                }
            }
        }


        async void UserInfoCall(string access_token)
        {
            //output("Making API Call to Userinfo...");

            // builds the  request
            string userinfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";

            // sends the request
            HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            // gets the response
            WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
            using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
            {
                // reads response body
                string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
                output(userinfoResponseText);
            }
        }

        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="outputStr">string to be appended</param>
        public async void output(string outputStr)
        {
            int subInd = outputStr.IndexOf("\"sub\"", StringComparison.OrdinalIgnoreCase);
            
            if (subInd!=0)
            {
                string subID = outputStr.Substring(subInd + 8, 21);
                SubID = subID;
                LoginVM loginVM = new LoginVM();
                await loginVM.LoginAsync(subID);
            }
            
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        public static string RandomDataBase64URL(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64URLEncodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputStirng"></param>
        /// <returns></returns>
        public static byte[] SHA256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string Base64URLEncodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

    }
}
