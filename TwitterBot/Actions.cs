using MV.Twitter.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MV.Twitter.TwitterBot
{
    public class GetStatAction : UserTimeline
    {
        public CharStatModel Model { get; protected set; }

        public int Precision { get; set; }
        public string StatText { get; protected set; }
        public string StatTextForTweet
        {
            get
            {
                var prefix = String.Format(@"{0}, cтатистика для последних 5 твитов: ", Username.StartsWith("@") ? Username : "@" + Username);
                var stat = Trim(StatText, 280 - prefix.Length);
                return prefix + stat;
            }
        }
        public bool IsOk { get { return Code == HttpStatusCode.OK; } }
        public string Error
        {
            get
            {
                switch(Code)
                {
                    case default(HttpStatusCode): return String.Empty;
                    case HttpStatusCode.OK: return String.Empty;
                    case HttpStatusCode.Unauthorized: return "Unauthorized. Please check keys";
                    case HttpStatusCode.Forbidden: return "Forbidden. Please check permissions for application";
                    case HttpStatusCode.NotFound: return String.Format(@"User [{0}] not found", Username);
                    default: return String.Format(@"Unexpeted error. Code is {0}",Code);
                }
            }
        }

        public GetStatAction()
        {
            Precision = 3;
        }

        public override void SetResponse(System.Net.HttpStatusCode code, string content)
        {
            base.SetResponse(code, content);

            if (code == System.Net.HttpStatusCode.OK)
            {
                Model = CharStat.GetStatModel(
                    string.Concat(
                    Json.Deserialize<List<MinModel.Status>>(RawContent)
                    .Select(x => x.full_text)
                    ));
                StatText = Model.ToJsonString(Precision);
            }
        }
        public static string Trim(string text, int limit)
        {
            if (text.Length <= limit) return text;
            var commaIndex = text.Substring(0, limit).LastIndexOf(',');
            if (commaIndex < 0) return "{}";
            return text.Substring(0, commaIndex) + "}";
        }
    }

    public class TweetStatAction : StatusesUpdate
    {
        public bool IsOk { get { return Code == HttpStatusCode.OK; } }
        public string Error
        {
            get
            {
                switch (Code)
                {
                    case default(HttpStatusCode): return String.Empty;
                    case HttpStatusCode.OK: return String.Empty;
                    case HttpStatusCode.Unauthorized: return "Unauthorized. Please check keys";
                    case HttpStatusCode.Forbidden: return "Forbidden. Probably you try to tweet text already tweeted earlier. Otherwise, please check permissions for application";
                    default: return String.Format(@"Unexpeted error. Code is {0}", Code);
                }
            }
        }
    }
}
