using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MV.Tests
{
    public static class Settings
    {
        public static string SEARCH_URI = @"https://api.twitter.com/1.1/search/tweets.json?q=Mike__Void&result_type=recent&count=5";
        public static string AUTH_URI = @"https://api.twitter.com/oauth2/token";

        public static string oauth_consumer_key { get { return MV.Tests.Properties.Settings.Default.oauth_consumer_key; } }
        public static string oauth_token { get { return MV.Tests.Properties.Settings.Default.oauth_token; } }
        public static string consumer_secret { get { return MV.Tests.Properties.Settings.Default.consumer_secret; } }
        public static string token_secret { get { return MV.Tests.Properties.Settings.Default.token_secret; } }
        public static string usernameBad { get { return "@bankTo567567chka"; } }
        public static string usernameMe { get { return "Mike__Void"; } }
        public static string usernameA { get { return "@bankTochka"; } }
        public static string username { get { return "bankTochka"; } }
    }
}
