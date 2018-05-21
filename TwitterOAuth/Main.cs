using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MV.Twitter.Auth
{
    public interface ITwitterOAuth
    {
        string GetAuthHeader(string uri, string format, HttpMethod method);
        string GetAuthAppHeader();
    }

    public class TwitterOAuth : ITwitterOAuth
    {
        protected string _consumer_secret;
        protected string _token_secret;

        protected SortedDictionary<string, string> oAuthParams = new SortedDictionary<string, string>();

        /// <summary>
        /// Creates headers for Twitter API authentification
        /// </summary>
        /// <param name="consumer_key">application public key</param>
        /// <param name="token">public token</param>
        /// <param name="consumer_secret">application secret key</param>
        /// <param name="token_secret">secret token</param>
        /// <param name="signature_method">HMAC-SHA1 is default</param>
        /// <param name="version">1.0 is default</param>
        public TwitterOAuth(string consumer_key, string token, string consumer_secret, string token_secret, string signature_method = "HMAC-SHA1", string version = "1.0")
        {
            oAuthParams = new SortedDictionary<string, string>()
            {
                {"oauth_consumer_key", consumer_key},
                {"oauth_nonce", NewNonce()},
                {"oauth_signature_method", signature_method},
                {"oauth_timestamp", Timestamp()},
                {"oauth_token", token},
                {"oauth_version", version}
            };
            _consumer_secret = consumer_secret;
            _token_secret = token_secret;
        }

        /// <summary>
        /// Creates signed authorization header for given uri
        /// </summary>
        /// <param name="uri">Valid uri with escaped parameters expected</param>
        /// <param name="format">String format for output, i.e. "OAuth {0}"</param>
        /// <param name="method">Default method is GET</param>
        /// <returns>Header content with signature</returns>
        public string GetAuthHeader(string uri, string format, HttpMethod method)
        {
            oAuthParams["oauth_nonce"] = NewNonce();
            oAuthParams["oauth_timestamp"] = Timestamp();
            var OAuthParams = new SortedDictionary<string, string>(oAuthParams);
            var SignParams = new SortedDictionary<string, string>(oAuthParams);
            var pQuery = HttpUtility.ParseQueryString(new Uri(Uri.EscapeUriString(uri)).Query);
            foreach(var key in pQuery.AllKeys)
            {
                SignParams.Add(Uri.UnescapeDataString(key), Uri.UnescapeDataString(pQuery[key]));
            }
            var paramString = string.Join("&",
                SignParams.Keys.Select(key =>
                    String.Format(@"{0}={1}",
                    Uri.EscapeDataString(key),
                    pQuery.AllKeys.Contains(key) ? EscapeDataString(SignParams[key]) : Uri.EscapeDataString(SignParams[key])
                    )));
            var baseString = String.Format(@"{0}&{1}&{2}",
                (method ?? HttpMethod.Get).Method.ToUpper(),
                Uri.EscapeDataString(new UriBuilder(uri) { Query = String.Empty }.Uri.AbsoluteUri),
                Uri.EscapeDataString(paramString)
                );
            var signingKey = String.Format(@"{0}&{1}",
                _consumer_secret,
                _token_secret
                );
            var signature = Convert.ToBase64String(
                new HMACSHA1(Encoding.ASCII.GetBytes(signingKey))
                .ComputeHash(Encoding.ASCII.GetBytes(baseString))
                );
            OAuthParams.Add("oauth_signature", signature);
            var header = string.Join(", ",
                OAuthParams.Select(x => String.Format(@"{0}=""{1}""",
                    Uri.EscapeDataString(x.Key),
                    Uri.EscapeDataString(x.Value)
                    )));
            return String.Format(format,header);
        }

        /// <summary>
        /// Authentification header for application-only authorization 
        /// </summary>
        /// <returns></returns>
        public string GetAuthAppHeader()
        {
            return Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    String.Format(@"{0}:{1}",
                        Uri.EscapeDataString(oAuthParams["oauth_consumer_key"]),
                        Uri.EscapeDataString(_consumer_secret)
                        )));
        }

        /// <summary>
        /// Generates random hex-string data
        /// </summary>
        /// <returns></returns>
        public static string NewNonce()
        {
            return Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// Total seconds from 1/1/1970
        /// </summary>
        /// <returns></returns>
        public static string Timestamp()
        {
            return ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        protected string EscapeDataString(string data)
        {
            return new Uri("http://a?" + Uri.EscapeDataString(data)).AbsoluteUri.Substring(10);            
        }
    }
}
