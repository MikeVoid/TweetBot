using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MV.Twitter.API
{
    public static class Uris
    {
        public static string AuthorizeApp = "https://api.twitter.com/oauth2/token";
        public static string SearchTweets = "https://api.twitter.com/1.1/search/tweets.json";
        public static string UserTimelne = "https://api.twitter.com/1.1/statuses/user_timeline.json";
        public static string StatusesShow = "https://api.twitter.com/1.1/statuses/show.json";
        public static string StatusesUpdate = "https://api.twitter.com/1.1/statuses/update.json";
    }
}
