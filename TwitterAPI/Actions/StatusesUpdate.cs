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
    public class StatusesUpdate : ApiAction
    {
        public string Status { get; set; }
        public string InReplyId { get; set; }

        public StatusesUpdate()
        {
            ApiUri = Uris.StatusesUpdate;
            Method = HttpMethod.Post;
        }

        public override string GetRequest()
        {
            return ApiUri
                + "?status=" + Uri.EscapeDataString(Status)
                 + (!string.IsNullOrEmpty(InReplyId) ? "&in_reply_to_status_id=" + InReplyId : String.Empty);
        }

        public override void SetResponse(HttpStatusCode code, string content)
        {
            Code = code;
            RawContent = content;
        }
    }
}
