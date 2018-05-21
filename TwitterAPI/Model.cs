using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MV.Twitter.API
{
    /// <summary>
    /// A very basic model for deserializing API response
    /// </summary>
    public class MinModel
    {
        [DataContract]
        public class Statuses
        {
            [DataMember]
            public List<Status> statuses { get; set; }
        }

        [DataContract]
        public class Status
        {
            [DataMember]
            public string text { get; set; }
            [DataMember]
            public string full_text { get; set; }
            [DataMember]
            public string id_str { get; set; }
        }
    }
}
