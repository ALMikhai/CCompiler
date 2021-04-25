using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    public partial class TokenizerTests
    {
        [TestMethod]
        public void StandardIdentifier()
        {
            const string fileName = "StandardIdentifier";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void IdentifierNumbers()
        {
            const string fileName = "IdentifierNumbers";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void IdentifierUnderline()
        {
            const string fileName = "IdentifierUnderline";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}