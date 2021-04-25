using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    public partial class TokenizerTests
    {
        [TestMethod]
        public void AllOperators()
        {
            const string fileName = "AllOperators";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}