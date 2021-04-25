using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ParserTests
{
    public partial class ParserTests
    {
        [TestMethod]
        public void CastExp()
        {
            const string fileName = "CastExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}