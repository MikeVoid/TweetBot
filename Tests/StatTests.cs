using Microsoft.VisualStudio.TestTools.UnitTesting;
using MV.Twitter.TwitterBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MV.Tests
{
    [TestClass]
    public class StatTests
    {
        [TestMethod, TestCategory("Stat")]
        public void GetStat_Correct()
        {
            var stat = CharStat.GetStat("test");
            var check = stat.Count == 3
                && (((int)Math.Round(stat['t'] * 100)) == 50)
                && (((int)Math.Round(stat['e'] * 100)) == 25)
                && (((int)Math.Round(stat['s'] * 100)) == 25);
            Assert.IsTrue(check);
        }

        [TestMethod, TestCategory("Stat")]
        public void GetStatEmptyString_ListCountIsZero()
        {
            var stat = CharStat.GetStat("");
            Assert.AreEqual(stat.Count, 0);
        }

        [TestMethod, TestCategory("Stat")]
        public void GetStatMultipleLanguage_Correct()
        {
            var stat = CharStat.GetStat("夢林玄解 維基電子版 幾乎全部都是亂1someother-text2что-тонарусском1");
            Assert.AreEqual(stat.Count, 34);
        }

        [TestMethod, TestCategory("Stat")]
        public void GetStatNull_ListCountIsZero()
        {
            var stat = CharStat.GetStat(null);
            Assert.AreEqual(stat.Count, 0);
        }
                
        [TestMethod,TestCategory("Stat")]
        public void GetStatModel_Correct()
        {
            var stat = CharStat.GetStatModel("test");
            stat.Pairs.Single(x => x.Char == 't' && ((int)Math.Round(x.Frequency * 100)) == 50);
            stat.Pairs.Single(x => x.Char == 'e' && ((int)Math.Round(x.Frequency * 100)) == 25);
            stat.Pairs.Single(x => x.Char == 's' && ((int)Math.Round(x.Frequency * 100)) == 25);
            Assert.AreEqual(stat.Pairs.Count, 3);
        }

        [TestMethod, TestCategory("Stat")]
        public void GetStatModelEmptyString_ListCountIsZero()
        {
            var stat = CharStat.GetStatModel("");
            Assert.AreEqual(stat.Pairs.Count, 0);
        }

        [TestMethod, TestCategory("Stat")]
        public void GetStatModelMultipleLanguage_Correct()
        {
            var stat = CharStat.GetStatModel("夢林玄解 維基電子版 幾乎全部都是亂1someother-text2что-тонарусском1");
            Assert.AreEqual(stat.Pairs.Count, 34);
        }

        [TestMethod, TestCategory("Stat")]
        public void GetStatModelNull_ListCountIsZero()
        {
            var stat = CharStat.GetStatModel(null);
            Assert.AreEqual(stat.Pairs.Count, 0);
        }

        [TestMethod, TestCategory("Stat")]
        public void GetStatModelFrequencySortCheck_Correct()
        {
            var stat = CharStat.GetStatModel("abbbcc");
            Assert.IsTrue(stat.Pairs[0].Char == 'b' && stat.Pairs[2].Char == 'a');
        }

        [TestMethod, TestCategory("Stat")]
        public void GetStatModelAlphabeticalSortCheck_Correct()
        {
            var stat = CharStat.GetStatModel("abbbcc", true);
            Assert.IsTrue(stat.Pairs[0].Char == 'a' && stat.Pairs[2].Char == 'c');
        }

        [TestMethod, TestCategory("Stat")]
        public void FilterTo_LettersLowCase()
        {
            var filtered = CharStat.Filter("-*‡€⁋™Жקओを\U0001F000\U0001D49E");
            Assert.AreEqual(filtered, "жקओを");
        }

        [TestMethod, TestCategory("Stat")]
        public void FilterTo_LettersAndDigits()
        {
            var filtered = CharStat.Filter("d*34- 4 ½ sfd 29€8sa", CharStat.CharStatOptions.IncludeDigits);
            Assert.AreEqual(filtered, "d344sfd298sa");
        }

        [TestMethod, TestCategory("Stat")]
        public void FilterTo_LettersAndWhitespaces()
        {
            var filtered = CharStat.Filter("d*34- 4 ½ sfd 29€8sa", CharStat.CharStatOptions.IncludeWhiteSpaces);
            Assert.AreEqual(filtered, "d   sfd sa");
        }

        [TestMethod, TestCategory("Stat")]
        public void FilterTo_LettersAndSymbols()
        {
            var filtered = CharStat.Filter("d*34- 4 ½ sfd 29€8sa", CharStat.CharStatOptions.IncludeSymbols);
            Assert.AreEqual(filtered, "d*-½sfd€sa");
        }

        [TestMethod, TestCategory("Stat")]
        public void FilterTo_NonDigits()
        {
            var filtered = CharStat.Filter("d*34- 4 ½ sfd 29€8sa", CharStat.CharStatOptions.IncludeSymbols | CharStat.CharStatOptions.IncludeWhiteSpaces);
            Assert.AreEqual(filtered, "d*-  ½ sfd €sa");
        }

        [TestMethod, TestCategory("Stat")]
        public void FilterTo_NonSymbols()
        {
            var filtered = CharStat.Filter("d*34- 4 ½ sfd 29€8sa", CharStat.CharStatOptions.IncludeDigits | CharStat.CharStatOptions.IncludeWhiteSpaces);
            Assert.AreEqual(filtered, "d34 4  sfd 298sa");
        }

        [TestMethod, TestCategory("Stat")]
        public void FilterTo_NonWhitespaces()
        {
            var filtered = CharStat.Filter("d*34- 4 ½ sfd 29€8sa", CharStat.CharStatOptions.IncludeDigits | CharStat.CharStatOptions.IncludeSymbols);
            Assert.AreEqual(filtered, "d*34-4½sfd29€8sa");
        }
    }
}
