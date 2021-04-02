using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
