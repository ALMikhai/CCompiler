using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ParserTests
{
    public partial class ParserTests
    {
        [TestMethod]
        public void MultExp()
        {
            const string fileName = "MultExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}