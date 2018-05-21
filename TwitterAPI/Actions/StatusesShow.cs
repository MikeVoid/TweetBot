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
    public class StatusesShow : ApiAction
    {
        public string TweetId { get; set; }
        public bool TakeRecent { get; set; }

        public StatusesShow()
        {
            ApiUri = Uris.StatusesShow;
            Method = HttpMethod.Get;
        }

        public override string GetRequest()
        {
            return ApiUri
                + "?id=" + TweetId
                + "&tweet_mode=extended";
        }

        public override void SetResponse(HttpStatusCode code, string content)
        {
            Code = code;
            RawContent = content;
        }
    }    
}
