using Microsoft.VisualStudio.TestTools.UnitTesting;
using MV.Twitter.API;
using MV.Twitter.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace MV.Tests
{
    [TestClass]
    public class ApiTests
    {
        static ITwitterApi Api = new TwitterApi(Settings.oauth_consumer_key, Settings.oauth_token, Settings.consumer_secret, Settings.token_secret);

        #region API basics

        [TestMethod, TestCategory("API")]
        public async Task SignedStatusesUpdate_GotContent()
        {
            var query = new StatusesUpdate() { Status = String.Format("{0}{2}{1}","@Mike__Void", TwitterOAuth.NewNonce(), Environment.NewLine), InReplyId = "996346241826131969" };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API")]
        public async Task SignedSearchTweets_GotContent()
        {
            var query = new SearchTweets() { Username = Settings.username, TweetsCount = 5, TakeRecent = true };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API")]
        public async Task SignedUserTimeline_GotContent()
        {
            var query = new UserTimeline() { Username = Settings.username, TweetsCount = 5 };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API")]
        public async Task SignedStatusesShow_GotContent()
        {
            var query = new StatusesShow() { TweetId = "994560027955466241" };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API")]
        public async Task SignedStatusesUpdate2_GotContent()
        {
            var query = new StatusesUpdate() { Status = TwitterOAuth.NewNonce() + "asd3dd{dd:ggпggg" };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API")]
        public async Task AppOnlySearchTweets_GotContent()
        {
            await Api.AuthorizeApp();
            var query = new SearchTweets() { Username = Settings.username, TweetsCount = 5, TakeRecent = true };
            await Api.Send(query, true);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API")]
        public async Task AppOnlyUserTimeline_GotContent()
        {
            await Api.AuthorizeApp();
            var query = new UserTimeline() { Username = "bankTochka", TweetsCount = 5 };
            await Api.Send(query, true);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API")]
        public async Task AppOnlyStatusesShow_GotContent()
        {
            await Api.AuthorizeApp();
            var query = new StatusesShow() { TweetId = "994560027955466241" };
            await Api.Send(query, true);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API")]
        public async Task AuthApp_GotToken()
        {
            await Api.AuthorizeApp();
            Assert.IsTrue(!string.IsNullOrEmpty(Api.AccessToken));
        }

        #endregion

        #region API Behaviour

        [TestMethod, TestCategory("API.Behaviour")]
        public async Task GetTweetsCorrectUserNameWithA_GotContent()
        {
            var query = new UserTimeline() { Username = Settings.usernameA, TweetsCount = 5 };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API.Behaviour")]
        public async Task GetTweetsCorrectUserNameWithoutA_GotContent()
        {
            var query = new UserTimeline() { Username = Settings.username, TweetsCount = 5 };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API.Behaviour")]
        public async Task GetTweetsIncorrectUserName_NotFound()
        {
            var query = new UserTimeline() { Username = Settings.usernameBad, TweetsCount = 5 };
            await Api.Send(query, false);
            Assert.IsTrue(query.Code == System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod, TestCategory("API.Behaviour")]
        public async Task PostTweetOver280_Forbidden()
        {
            var query = new StatusesUpdate() { Status = GenerateText(281, true) };
            await Api.Send(query, false);
            Assert.IsTrue(query.Code == System.Net.HttpStatusCode.Forbidden);
        }

        [TestMethod, TestCategory("API.Behaviour")]
        public async Task PostTweetOver280Escape_GotContent()
        {
            var query = new StatusesUpdate() { Status = GenerateText(279, false) + ',' };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API.Behaviour")]
        public async Task PostTweet280_GotContent()
        {
            var query = new StatusesUpdate() { Status = GenerateText(280, true) };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }

        [TestMethod, TestCategory("API.Behaviour")]
        public async Task PostTweet280Ru_GotContent()
        {
            var query = new StatusesUpdate() { Status = GenerateText(280, false) };
            await Api.Send(query, false);
            Assert.IsTrue(!string.IsNullOrEmpty(query.RawContent));
        }        

        [TestMethod]
        public void CheckGenerator()
        {
            Assert.IsTrue(GenerateText(281, true).Length == 281, "281");
            Assert.IsTrue(GenerateText(280, true).Length == 280, "280");
            Assert.IsTrue(GenerateText(279, true).Length == 279, "279");
            Assert.IsTrue(GenerateText(1, true).Length == 1, "1");
            Assert.IsTrue(GenerateText(10, true).Length == 10, "10");
            Assert.IsTrue(GenerateText(0, true).Length == 0, "0");
        }

        string GenerateText(int size, bool en)
        {            
            string output = String.Empty;
            var digits = size.ToString().Length;
            var chars = 10 - digits;
            var tail = size % 10;
            for (int i = 1; i < size-tail; i += 10)
            {
                output += i.ToString("D"+digits.ToString()) + new String(en ? RandLetterEn : RandLetterRu, chars);
            }
            if (tail != 0) output += new String('t', tail);
            return output;
        }
        Random rand = new Random();
        char[] lettersEn = Enumerable.Range('A', 26).Select(x => (char)x).ToArray();
        char[] lettersRu = Enumerable.Range('А', 33).Select(x => (char)x).ToArray();
        char RandLetterEn { get { return lettersEn[rand.Next(26)]; } }
        char RandLetterRu { get { return lettersRu[rand.Next(33)]; } }

        #endregion
        
    }
}
