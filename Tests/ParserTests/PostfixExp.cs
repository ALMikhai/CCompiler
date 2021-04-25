using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ParserTests
{
    public partial class ParserTests
    {
        [TestMethod]
        public void EmptyBracketsPostfixExp()
        {
            const string fileName = "EmptyBracketsPostfixExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
        
        [TestMethod]
        public void DotPostfixExp()
        {
            const string fileName = "DotPostfixExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
        
        [TestMethod]
        public void ArrowPostfixExp()
        {
            const string fileName = "ArrowPostfixExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
        
        [TestMethod]
        public void IncPostfixExp()
        {
            const string fileName = "IncPostfixExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
        
        [TestMethod]
        public void DecPostfixExp()
        {
            const string fileName = "DecPostfixExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}