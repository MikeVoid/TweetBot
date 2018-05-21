using Microsoft.VisualStudio.TestTools.UnitTesting;
using MV.Twitter.API;
using MV.Twitter.TwitterBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MV.Tests
{
    [TestClass]
    public class Tests
    {
        static ITwitterApi Api = new TwitterApi(Settings.oauth_consumer_key, Settings.oauth_token, Settings.consumer_secret, Settings.token_secret);

        GetStatAction GetStat = new GetStatAction() { TweetsCount = 5, Precision = 3 };
        TweetStatAction PostTweet = new TweetStatAction();

        [TestMethod, TestCategory("Main")]
        public void Common_Correct()
        {
            GetStat.Username = Settings.username;
            Api.Send(GetStat, false).Wait();
            Assert.IsTrue(GetStat.IsOk, "get fails");
            PostTweet.Status = MakeUnique(GetStat.StatTextForTweet);
            Api.Send(PostTweet, false).Wait();
            Assert.IsTrue(PostTweet.IsOk, "post fails");
        }

        [TestMethod, TestCategory("Main")]
        public void GetStatWrongKeys_ErrorMessage()
        {
            var Api_backup = Api;
            Api = new TwitterApi(Settings.oauth_consumer_key, Settings.oauth_token, Settings.consumer_secret, "");
            GetStat.Username = Settings.username;
            Api.Send(GetStat, false).Wait();
            Api = Api_backup;
            Assert.IsTrue(GetStat.Error == "Unauthorized. Please check keys");
        }

        [TestMethod, TestCategory("Main")]
        public void PostTweetWrongKeys_ErrorMessage()
        {
            var Api_backup = Api;
            Api = new TwitterApi(Settings.oauth_consumer_key, Settings.oauth_token, Settings.consumer_secret, "");
            PostTweet.Status = MakeUnique("NoToken");
            Api.Send(PostTweet, false).Wait();
            Api = Api_backup;
            Assert.IsTrue(PostTweet.Error == "Unauthorized. Please check keys");
        }

        [TestMethod, TestCategory("Main")]
        public void GetStatWrongUsername_ErrorMessage()
        {
            GetStat.Username = Settings.usernameBad;
            Api.Send(GetStat, false).Wait();
            Assert.IsTrue(GetStat.Error == String.Format(@"User [{0}] not found", GetStat.Username));
        }

        [TestMethod, TestCategory("Main")]
        public void PostTweetDuplicate_ErrorMessage()
        {
            PostTweet.Status = MakeUnique("DuplicateTest");
            Api.Send(PostTweet, false).Wait();
            Assert.IsTrue(PostTweet.IsOk);
            Api.Send(PostTweet, false).Wait();
            Assert.IsTrue(PostTweet.Error == "Forbidden. Probably you try to tweet text already tweeted earlier. Otherwise, please check permissions for application");
        }

        public static string MakeUnique(string text, int limit = 280)
        {
            var unique = MV.Twitter.Auth.TwitterOAuth.NewNonce();
            var pos = Math.Max(unique.Length - (limit - text.Length), 0);
            return unique + text.Substring(pos);
        }
    }    
}
