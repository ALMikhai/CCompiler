﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class Int
    {
        [TestMethod]
        public void StandardInt()
        {
            const string fileName = "StandardInt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void OctalInt()
        {
            const string fileName = "OctalInt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void HexInt()
        {
            const string fileName = "HexInt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void LongInt()
        {
            const string fileName = "LongInt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void UnsignedInt()
        {
            const string fileName = "UnsignedInt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void UnsignedLongInt()
        {
            const string fileName = "UnsignedLongInt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void OctalErrorInt()
        {
            const string fileName = "OctalErrorInt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void OctalErrorInt2()
        {
            const string fileName = "OctalErrorInt2";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void HexErrorInt()
        {
            const string fileName = "HexErrorInt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void OverflowErrorInt()
        {
            const string fileName = "OverflowErrorInt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}