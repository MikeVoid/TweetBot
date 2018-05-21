using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using MV.Twitter.Auth;
using System.Collections.Generic;

namespace MV.Tests
{
    [TestClass]
    public class AuthTests
    {
        static ITwitterOAuth Auth = new TwitterOAuth(Settings.oauth_consumer_key, Settings.oauth_token, Settings.consumer_secret, Settings.token_secret);

        [TestMethod, TestCategory("Auth")]
        public async Task SignedSearchTweets_HttpOk()
        {
            HttpClient client = new HttpClient();
            string header = Auth.GetAuthHeader(Settings.SEARCH_URI, "OAuth {0}", HttpMethod.Get);
            HttpRequestMessage request = new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = new Uri(Settings.SEARCH_URI) };
            request.Headers.Add("Authorization", header);
            var result = await client.SendAsync(request).ConfigureAwait(false);
            Assert.IsTrue(result.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod, TestCategory("Auth")]
        public async Task AuthAppRequest_HttpOk()
        {
            HttpClient client = new HttpClient();
            string header = Auth.GetAuthAppHeader();
            HttpRequestMessage request = new HttpRequestMessage() { Method = HttpMethod.Post, RequestUri = new Uri(Settings.AUTH_URI) };
            request.Headers.Add("Authorization", "Basic " + header);
            request.Content = new System.Net.Http.FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });
            request.Headers.Add("Accept-Encoding", "gzip");
            var result = await client.SendAsync(request).ConfigureAwait(false);
            Assert.IsTrue(result.StatusCode == HttpStatusCode.OK);
        }

    }
}
