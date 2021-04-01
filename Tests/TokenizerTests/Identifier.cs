using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class Identifier
    {
        [TestMethod]
        public void StandardIdentifier()
        {
            const string fileName = "StandardIdentifier.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void IdentifierNumbers()
        {
            const string fileName = "IdentifierNumbers.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void IdentifierUnderline()
        {
            const string fileName = "IdentifierUnderline.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}
