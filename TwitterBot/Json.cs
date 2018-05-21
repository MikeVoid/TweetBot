using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace MV.Twitter.TwitterBot
{
    public class Json
    {
        public static T Deserialize<T>(string data) where T:class
        {
            T deserialized = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(deserialized.GetType());
                deserialized = ser.ReadObject(ms) as T;
            }
            return deserialized;
        }

        public static string Serialize(object model, bool UseSimpleDictionaryFormat = false)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var settings = new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = UseSimpleDictionaryFormat };
                DataContractJsonSerializer ser = new DataContractJsonSerializer(model.GetType(), settings);
                ser.WriteObject(ms, model);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
