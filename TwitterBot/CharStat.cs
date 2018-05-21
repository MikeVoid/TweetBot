using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MV.Twitter.TwitterBot
{
    public class CharStat
    {
        /// <summary>
        /// Returns char frequency for given text
        /// </summary>
        /// <param name="text">Source text for statistic</param>
        /// <param name="alphabetical">Sort order, by frequency descendant or alphabetical ascending</param>
        /// <param name="options">Text filtering options, see CharStat.Filter</param>
        /// <returns>Serializable model with ordered pairs</returns>
        public static CharStatModel GetStatModel(string text, bool alphabetical = false, CharStatOptions options = CharStatOptions.None)
        {
            var source = Filter(text, options);
            var query = source
                    .GroupBy(
                        x => x,
                        x => 1.0,
                        (c, g) => new CharPair()
                        {
                            Char = c,
                            Frequency = g.Sum() / source.Length
                        });
            if (alphabetical) query = query.OrderBy(x => x.Char);
            else query = query.OrderByDescending(x => x.Frequency);
            return new CharStatModel() { Pairs = query.ToList() };
        }

        /// <summary>
        /// Returns char frequency for given text
        /// </summary>
        /// <param name="text">Source text for statistic</param>
        /// <param name="options">Text filtering options, see CharStat.Filter</param>
        /// <returns>Dictionary, where char is key</returns>
        public static Dictionary<char, double> GetStat(string text, CharStatOptions options = CharStatOptions.None)
        {
            var source = Filter(text, options);
            return source
                .GroupBy(
                        x => x,
                        x => 1.0,
                        (c, g) => new
                        {
                            Char = c,
                            Frequency = g.Sum() / source.Length
                        })
                    .ToDictionary(x => x.Char, x => x.Frequency);
        }

        /// <summary>
        /// Filters given string according to options. Surrogate pairs ignored
        /// </summary>
        /// <param name="text">Source string to filter, null string counts as empty</param>
        /// <param name="options">Default is letters in lower case</param>
        /// <returns>Filtered string, not null</returns>
        public static string Filter(string text, CharStatOptions options = CharStatOptions.None)
        {
            var source = text ?? String.Empty;
            if(!options.HasFlag(CharStatOptions.CaseSensetive)) source = source.ToLowerInvariant();
            return String.Concat(
                source.Where(c => 
                {
                    if (char.IsWhiteSpace(c)) return options.HasFlag(CharStatOptions.IncludeWhiteSpaces);
                    if (char.IsDigit(c)) return options.HasFlag(CharStatOptions.IncludeDigits);
                    return options.HasFlag(CharStatOptions.IncludeSymbols) || char.IsLetter(c);
                }));
        }

        [Flags]
        public enum CharStatOptions
        {
            None = 0,
            CaseSensetive = 1,
            IncludeWhiteSpaces = 2,
            IncludeSymbols = 4,
            IncludeDigits = 8
        }
    }

    [DataContract]
    public class CharStatModel
    {
        [DataMember]
        public List<CharPair> Pairs { get; set; }

        public OrderedDictionary AsOrderedDictionary(int roundPrecision = -1)
        {
            var dictionary = new OrderedDictionary();
            if (Pairs != null)
            {
                foreach (var pair in Pairs)
                {
                    dictionary.Add(pair.Char, roundPrecision == -1 ? pair.Frequency : Math.Round(pair.Frequency, roundPrecision));
                }
            }
            return dictionary;
        }

        public string ToJsonString(int roundPrecision = -1)
        {
            return
                "{"
                + string.Join(", ",
                Pairs.Select(
                x => String.Format(@"{0}:'{1}'",
                    x.Char,
                    roundPrecision == -1 ? x.Frequency : Math.Round(x.Frequency, roundPrecision)
                    )))
                + "}";
        }
    }
    [DataContract]
    public class CharPair
    {
        [DataMember]
        public char Char { get; set; }
        [DataMember]
        public double Frequency { get; set; }
    }
}
