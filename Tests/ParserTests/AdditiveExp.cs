using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.ParserTests
{
    public partial class ParserTests
    {
        [TestMethod]
        public void AdditiveExp()
        {
            const string fileName = "AdditiveExp";
            Assert.IsTrue(Utils.CheckCorrect(DirectoryPath, fileName, CompileKeys));
        }
    }
}