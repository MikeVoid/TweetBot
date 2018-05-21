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
    public class UserTimeline : ApiAction
    {
        public string Username { get; set; }
        public int TweetsCount { get; set; }
        
        public UserTimeline()
        {
            ApiUri = Uris.UserTimelne;
            Method = HttpMethod.Get;
        }

        public override string GetRequest()
        {
            return ApiUri
                + "?screen_name=" + Uri.EscapeDataString(Username)
                + "&exclude_replies=true"
                + ((TweetsCount > 0) ? ("&count=" + TweetsCount) : String.Empty)
                + "&tweet_mode=extended";
        }

        public override void SetResponse(HttpStatusCode code, string content)
        {
            Code = code;
            RawContent = content;
        }
    }
}
