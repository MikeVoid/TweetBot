using MV.Twitter.API;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MV.Twitter.TwitterBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TwitterApi Api;
            try
            {
                var appSettings = MV.Twitter.TwitterBot.Properties.Settings.Default;
                string oauth_consumer_key = appSettings.oauth_consumer_key;
                string oauth_token = appSettings.oauth_token;
                string consumer_secret = appSettings.consumer_secret;
                string token_secret = appSettings.token_secret;

                if(string.IsNullOrEmpty(string.Concat(oauth_consumer_key, oauth_token, consumer_secret, token_secret)))
                {
                    Console.WriteLine("Please set the keys in settings");
                    Console.ReadLine();
                    return;
                }

                Api = new TwitterApi(oauth_consumer_key, oauth_token, consumer_secret, token_secret);
            }
            catch
            {
                Console.WriteLine("Can't read settings");
                Console.ReadLine();
                return;
            }
            
            var GetStat = new GetStatAction() { TweetsCount = 5, Precision = 3 };
            var PostTweet = new TweetStatAction();

            while(true)
            {
                Console.WriteLine("Enter a valid username to get char frequency statistic and tweet it:");
                var input = Console.ReadLine();
                if (input == String.Empty) break;
                GetStat.Username = input;
                Api.Send(GetStat, false).Wait();
                if (GetStat.IsOk)
                {
                    PostTweet.Status = GetStat.StatTextForTweet;
                    Api.Send(PostTweet, false).Wait();
                    if (PostTweet.IsOk)
                    {
                        foreach(var pair in GetStat.Model.Pairs)
                        {
                            Console.WriteLine(String.Format(@"'{0}': {1}", pair.Char, pair.Frequency));
                        }                        
                    }
                    else Console.WriteLine(PostTweet.Error);
                }
                else Console.WriteLine(GetStat.Error);
            }
        }        
    }
}
