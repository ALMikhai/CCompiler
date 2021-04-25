using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    public partial class TokenizerTests
    {
        [TestMethod]
        public void AllKeywords()
        {
            const string fileName = "AllKeywords";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}