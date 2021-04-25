using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ParserTests
{
    public partial class ParserTests
    {
        [TestMethod]
        public void IdPrimaryExp()
        {
            const string fileName = "IdPrimaryExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
        
        [TestMethod]
        public void StringPrimaryExp()
        {
            const string fileName = "StringPrimaryExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}