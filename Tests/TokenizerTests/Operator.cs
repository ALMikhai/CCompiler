﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class Operator
    {
        [TestMethod]
        public void AllOperators()
        {
            const string fileName = "AllOperators";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}