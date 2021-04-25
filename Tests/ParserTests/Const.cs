using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ParserTests
{
    [TestClass]
    public partial class ParserTests
    {
        public static string DirectoryPath { get; } = @"..\..\..\ParserTests\Tests\";
        public static string CompileKeys { get; } = "-p";
        
        [TestMethod]
        public void IntConst()
        {
            const string fileName = "IntConst";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
        
        [TestMethod]
        public void CharConst()
        {
            const string fileName = "CharConst";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
        
        [TestMethod]
        public void FloatConst()
        {
            const string fileName = "CharConst";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}