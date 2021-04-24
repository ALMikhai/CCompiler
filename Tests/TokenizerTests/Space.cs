using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class Space
    {
        [TestMethod]
        public void Empty()
        {
            const string fileName = "Empty";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void Spaces()
        {
            const string fileName = "Space";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void LineBreak()
        {
            const string fileName = "LineBreak";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}