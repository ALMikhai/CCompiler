using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public partial class TokenizerTests
    {
        public static string DirectoryPath { get; } = @"..\..\..\TokenizerTests\Tests\";
        public static string CompileKeys { get; } = "-l";

        [TestMethod]
        public void OneLineComment()
        {
            const string fileName = "OneLineComment";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void MultipleLineComment()
        {
            const string fileName = "MultipleLineComment";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void UnterminatedComment()
        {
            const string fileName = "UnterminatedComment";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }

        [TestMethod]
        public void NestedComments()
        {
            const string fileName = "NestedComments";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}