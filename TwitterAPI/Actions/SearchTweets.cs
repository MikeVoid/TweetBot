using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MV.Twitter.API
{
    public class SearchTweets : ApiAction
    {
        public string Username { get; set; }
        public int TweetsCount { get; set; }
        public bool TakeRecent { get; set; }

        public SearchTweets()
        {
            ApiUri = Uris.SearchTweets;
            Method = HttpMethod.Get;
        }

        public override string GetRequest()
        {
            return ApiUri
                + "?q=" + Uri.EscapeDataString(Username)
                + ((TweetsCount > 0) ? ("&count=" + TweetsCount) : String.Empty)
                + (TakeRecent ? "&result_type=recent" : String.Empty)
                + "&tweet_mode=extended";
        }

        public override void SetResponse(HttpStatusCode code, string content)
        {
            Code = code;
            RawContent = content;
        }
    }    
}
