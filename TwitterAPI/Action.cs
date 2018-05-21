using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MV.Twitter.API
{
    public interface ITwitterApiAction
    {
        /// <summary>
        /// Provide request uri
        /// </summary>
        /// <returns></returns>
        string GetRequest();
        /// <summary>
        /// Response input point
        /// </summary>
        /// <param name="content">Response code and content as string, if code is OK</param>
        void SetResponse(HttpStatusCode code, string content);
        /// <summary>
        /// Http method to be used in request
        /// </summary>
        HttpMethod Method { get; }        
    }

    /// <summary>
    /// A very basic class to operate with API
    /// </summary>
    public abstract class ApiAction : ITwitterApiAction
    {
        public string ApiUri { get; set; }
        public HttpMethod Method { get; set; }
        public string RawContent { get; protected set; }
        public HttpStatusCode Code { get; protected set; }

        public abstract string GetRequest();
        public abstract void SetResponse(HttpStatusCode code, string content);
    }
}
