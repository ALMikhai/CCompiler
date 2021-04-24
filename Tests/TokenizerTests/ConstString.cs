using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class ConstString
    {
        [TestMethod]
        public void StandardString()
        {
            const string fileName = "StandardString";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void LString()
        {
            const string fileName = "LString";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void EscapeSequencesString()
        {
            const string fileName = "EscapeSequencesString";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void QuoteErrorString()
        {
            const string fileName = "QuoteErrorString";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}