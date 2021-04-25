using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    public partial class TokenizerTests
    {
        [TestMethod]
        public void StandardChar()
        {
            const string fileName = "StandardChar";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void EscapeSequencesChar()
        {
            const string fileName = "EscapeSequencesChar";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void OctalChar()
        {
            const string fileName = "OctalChar";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void HexChar()
        {
            const string fileName = "HexChar";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void LChar()
        {
            const string fileName = "LChar";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void QuoteErrorChar()
        {
            const string fileName = "QuoteErrorChar";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void OctalErrorChar()
        {
            const string fileName = "OctalErrorChar";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void HexErrorChar()
        {
            const string fileName = "HexErrorChar";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void LErrorChar()
        {
            const string fileName = "LErrorChar";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}