using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class ConstChar
    {
        [TestMethod]
        public void StandardChar()
        {
            const string fileName = "StandardChar.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void EscapeSequencesChar()
        {
            const string fileName = "EscapeSequencesChar.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void OctalChar()
        {
            const string fileName = "OctalChar.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void HexChar()
        {
            const string fileName = "HexChar.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void LChar()
        {
            const string fileName = "LChar.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void QuoteErrorChar()
        {
            const string fileName = "QuoteErrorChar.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void OctalErrorChar()
        {
            const string fileName = "OctalErrorChar.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void HexErrorChar()
        {
            const string fileName = "HexErrorChar.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void LErrorChar()
        {
            const string fileName = "LErrorChar.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}
