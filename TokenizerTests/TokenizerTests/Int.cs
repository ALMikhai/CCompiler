using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class Int
    {
        [TestMethod]
        public void StandardInt()
        {
            const string fileName = "StandardInt.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void OctalInt()
        {
            const string fileName = "OctalInt.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void HexInt()
        {
            const string fileName = "HexInt.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void LongInt()
        {
            const string fileName = "LongInt.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void UnsignedInt()
        {
            const string fileName = "UnsignedInt.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void UnsignedLongInt()
        {
            const string fileName = "UnsignedLongInt.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void OctalErrorInt()
        {
            const string fileName = "OctalErrorInt.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void OctalErrorInt2()
        {
            const string fileName = "OctalErrorInt2.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void HexErrorInt()
        {
            const string fileName = "HexErrorInt.txt";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}
