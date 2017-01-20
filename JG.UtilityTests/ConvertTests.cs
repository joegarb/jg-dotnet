using NUnit.Framework;
using JG.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JG.Utility.Tests
{
    [TestFixture()]
    public class ConvertTests
    {
        #region ToWords

        [Test()]
        public void ToWords_Negative()
        {
            Assert.AreEqual("one", Convert.ToWords(-1));
        }

        [Test()]
        public void ToWords_Zero()
        {
            Assert.AreEqual("zero", Convert.ToWords(0));
        }

        [Test()]
        public void ToWords_Ones()
        {
            Assert.AreEqual("five", Convert.ToWords(5));
        }

        [Test()]
        public void ToWords_Tens()
        {
            Assert.AreEqual("twenty", Convert.ToWords(20));
        }

        [Test()]
        public void ToWords_TensAndOnes()
        {
            Assert.AreEqual("twenty seven", Convert.ToWords(27));
        }

        [Test()]
        public void ToWords_Hundreds()
        {
            Assert.AreEqual("six hundred", Convert.ToWords(600));
        }

        [Test()]
        public void ToWords_HundredsAndTens()
        {
            Assert.AreEqual("six hundred forty", Convert.ToWords(640));
        }

        [Test()]
        public void ToWords_HundredsTensAndOnes()
        {
            Assert.AreEqual("six hundred thirty one", Convert.ToWords(631));
        }

        [Test()]
        public void ToWords_Thousands()
        {
            Assert.AreEqual("twenty thousand", Convert.ToWords(20000));
        }

        [Test()]
        public void ToWords_ThousandsAndHundreds()
        {
            Assert.AreEqual("four thousand five hundred", Convert.ToWords(4500));
        }

        [Test()]
        public void ToWords_HundredThousands()
        {
            Assert.AreEqual("nine hundred sixty five thousand thirty two", Convert.ToWords(965032));
        }

        [Test()]
        public void ToWords_Millions()
        {
            Assert.AreEqual("one million", Convert.ToWords(1000000));
        }

        [Test()]
        public void ToWords_Billions()
        {
            Assert.AreEqual("two billion fifty million six", Convert.ToWords(2050000006));
        }

        #endregion
    }
}