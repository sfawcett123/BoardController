using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoardManager.BoardManagerTests
{
    /// <exclude />
    [TestClass]
    public class HelperTest
    {
        /// <exclude />
        [TestMethod , TestCategory("TCP")]
        public void TestSerialize()
        {
            Dictionary<string, string> TestDictionary = new()
            {
                { "KEY", "VALUE" }
            };


            Assert.AreEqual("{\"KEY\":\"VALUE\"}", TestDictionary.Serialize());
        }

        /// <exclude />
        [TestMethod, TestCategory("TCP")]
        public void TestMergeEqual()
        {
            Dictionary<string, string> TestDictionaryOne = new()
            {
                { "KEY", "VALUE" }
            };

            Dictionary<string, string> TestDictionaryTwo = new()
            {
                { "KEY", "VALUE" }
            };

            var res = TestDictionaryOne.MergeLeft(TestDictionaryTwo).Serialize();

            Assert.AreEqual("{\"KEY\":\"VALUE\"}", res);
        }

        /// <exclude />
        [TestMethod, TestCategory("TCP")]
        public void TestMergeDiff()
        {
            Dictionary<string, string> TestDictionaryOne = new()
            {
                { "KEYONE", "VALUEONE" }
            };

            Dictionary<string, string> TestDictionaryTwo = new()
            {
                { "KEYTWO", "VALUETWO" }
            };

            var res = TestDictionaryOne.MergeLeft(TestDictionaryTwo).Serialize();

            Assert.AreEqual("{\"KEYONE\":\"VALUEONE\",\"KEYTWO\":\"VALUETWO\"}", res);
        }
    }
}
