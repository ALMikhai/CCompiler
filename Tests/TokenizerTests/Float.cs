using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    public partial class TokenizerTests
    {
        [TestMethod]
        public void StandardFloat()
        {
            const string fileName = "StandardFloat";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void LongFloat()
        {
            const string fileName = "LongFloat";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void FloatFloat()
        {
            const string fileName = "FloatFloat";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void DotErrorFloat()
        {
            const string fileName = "DotErrorFloat";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void EErrorFloat()
        {
            const string fileName = "EErrorFloat";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void SingErrorFloat()
        {
            const string fileName = "SingErrorFloat";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}