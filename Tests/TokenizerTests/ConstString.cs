using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class ConstString
    {
        [TestMethod]
        public void StandardString()
        {
            const string fileName = "StandardString.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void LString()
        {
            const string fileName = "LString.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void EscapeSequencesString()
        {
            const string fileName = "EscapeSequencesString.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void QuoteErrorString()
        {
            const string fileName = "QuoteErrorString.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}
