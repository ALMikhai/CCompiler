using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class Keyword
    {
        [TestMethod]
        public void AllKeywords()
        {
            const string fileName = "AllKeywords";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}