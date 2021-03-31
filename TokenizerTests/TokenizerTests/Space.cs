using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class Space
    {
        [TestMethod]
        public void Empty()
        {
            const string fileName = "Empty.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void Spaces()
        {
            const string fileName = "Space.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void LineBreak()
        {
            const string fileName = "LineBreak.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}
