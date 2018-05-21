using MV.Twitter.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MV.Twitter.API
{
    public interface ITwitterApi
    {        
        Task Send(ITwitterApiAction action, bool authAppOnly);
        Task AuthorizeApp();
        string AccessToken { get; } 
    }    

    public class TwitterApi : ITwitterApi
    {
        protected ITwitterOAuth authorization = null;
        protected HttpClient client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate });

        public string AccessToken { get; protected set; }

        /// <summary>
        /// Provides basic functionality for Twitter API
        /// </summary>
        /// <param name="consumer_key">application public key</param>
        /// <param name="token">public token</param>
        /// <param name="consumer_secret">application secret key</param>
        /// <param name="token_secret">secret token</param>
        /// <param name="signature_method">HMAC-SHA1 is default</param>
        /// <param name="version">1.0 is default</param>
        public TwitterApi(string consumer_key, string token, string consumer_secret, string token_secret, string signature_method = "HMAC-SHA1", string version = "1.0")
        {
            authorization = new TwitterOAuth(consumer_key, token, consumer_secret, token_secret, signature_method, version);
        }

        protected async Task<HttpResponseMessage> Request(string uri, HttpMethod method = null)
        {
            var _method = method ?? HttpMethod.Get;
            HttpRequestMessage request = new HttpRequestMessage() { Method = _method, RequestUri = new Uri(uri) };
            request.Headers.Add("Authorization", authorization.GetAuthHeader(uri, "OAuth {0}", _method));
            return await client.SendAsync(request).ConfigureAwait(false);
        }

        protected async Task<HttpResponseMessage> RequestApp(string uri, HttpMethod method = null)
        {
            var _method = method ?? HttpMethod.Get;
            HttpRequestMessage request = new HttpRequestMessage() { Method = _method, RequestUri = new Uri(uri) };
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken));
            return await client.SendAsync(request).ConfigureAwait(false);
        }

        protected async Task<HttpResponseMessage> RequestAuth(string uri = null, HttpMethod method = null)
        {
            var _method = method ?? HttpMethod.Post;
            var _uri = string.IsNullOrEmpty(uri) ? Uris.AuthorizeApp : uri;
            HttpRequestMessage request = new HttpRequestMessage(_method, _uri);
            request.Headers.Add("Authorization", "Basic " + authorization.GetAuthAppHeader());            
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Content = new System.Net.Http.FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });
            return await client.SendAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends requests to API and provides response to action object
        /// </summary>
        /// <param name="action">Determines what to send and takes response</param>
        /// <param name="authAppOnly">Application only or full signed</param>
        /// <returns></returns>
        public async Task Send(ITwitterApiAction action, bool authAppOnly)
        {
            if(action != null)
            {
                var response =
                    authAppOnly
                    ? await RequestApp(action.GetRequest(), action.Method)
                    : await Request(action.GetRequest(), action.Method);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    action.SetResponse(response.StatusCode, await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
                else
                {
                    action.SetResponse(response.StatusCode, null);
                }
            }
        }

        /// <summary>
        /// Requests application authorization and stores access token
        /// </summary>
        /// <returns></returns>
        public async Task AuthorizeApp()
        {
            var response = await RequestAuth();
            if(response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var decoded = System.Web.Helpers.Json.Decode<AuthResponse>(data);
                AccessToken = Uri.UnescapeDataString(decoded.access_token);
            }
        }

        public class AuthResponse
        {
            public string token_type { get; set; }
            public string access_token { get; set; }
        }        
    }

    
}
