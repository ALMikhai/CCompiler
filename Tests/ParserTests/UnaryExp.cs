using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ParserTests
{
    public partial class ParserTests
    {
        [TestMethod]
        public void IncUnaryExp()
        {
            const string fileName = "IncUnaryExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
        
        [TestMethod]
        public void DecUnaryExp()
        {
            const string fileName = "DecUnaryExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
        
        [TestMethod]
        public void UnaryOpUnaryExp()
        {
            const string fileName = "UnaryOpUnaryExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}