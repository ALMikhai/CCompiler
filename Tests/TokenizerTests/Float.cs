using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class Float
    {
        [TestMethod]
        public void StandardFloat()
        {
            const string fileName = "StandardFloat.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void LongFloat()
        {
            const string fileName = "LongFloat.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void FloatFloat()
        {
            const string fileName = "FloatFloat.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void DotErrorFloat()
        {
            const string fileName = "DotErrorFloat.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void EErrorFloat()
        {
            const string fileName = "EErrorFloat.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void SingErrorFloat()
        {
            const string fileName = "SingErrorFloat.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}
