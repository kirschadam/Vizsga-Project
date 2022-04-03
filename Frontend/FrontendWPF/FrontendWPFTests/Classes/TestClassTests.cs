using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrontendWPF.Classes.Tests
{
    [TestClass()]
    public class TestClassTests
    {
        [TestMethod()]
        public void CompareTestWhenEqual()
        {
            TestClass testClass = new TestClass();
            bool expected = true;
            bool actual = testClass.Compare(50, 50, op: "=");
            Assert.AreEqual(expected, actual, message: "A két érték nem egyenlő!");
        }

        [TestMethod()]
        public void CompareTestWhenGreater()
        {
            TestClass testClass = new TestClass();
            bool expected = true;
            bool actual = testClass.Compare(51, 50, op: ">");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CompareTestWhenGreaterOrEqual()
        {
            TestClass testClass = new TestClass();
            bool expected = true;
            bool actual = testClass.Compare(51, 50, op: ">=") && testClass.Compare(50, 50, op: ">=");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CompareTestWhenLess()
        {
            TestClass testClass = new TestClass();
            bool expected = true;
            bool actual = testClass.Compare(50, 51, op: "<");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CompareTestWhenLessOrEqual()
        {
            TestClass testClass = new TestClass();
            bool expected = true;
            bool actual = testClass.Compare(50, 51, op: "<=") && testClass.Compare(50, 50, op: "<=");
            Assert.AreEqual(expected, actual);
        }

    }
}