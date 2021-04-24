using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TokenizerTests
{
    [TestClass]
    public class Comment
    {
        [TestMethod]
        public void OneLineComment()
        {
            const string fileName = "OneLineComment";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void MultipleLineComment()
        {
            const string fileName = "MultipleLineComment";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void UnterminatedComment()
        {
            const string fileName = "UnterminatedComment";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }

        [TestMethod]
        public void NestedComments()
        {
            const string fileName = "NestedComments";
            Assert.IsTrue(Utils.CheckCorrect(fileName));
        }
    }
}