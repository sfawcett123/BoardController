using Microsoft.VisualStudio.TestTools.UnitTesting;
using BoardManager;

namespace BoardManagerTests
{
    /// <exclude />
    [TestClass]
    public class HelperTest
    {
        /// <exclude />
        [TestMethod]
        public void TestSerialize()
        {
            Dictionary<string, string> TestDictionary = new()
            {
                { "KEY", "VALUE" }
            };

            
            Assert.AreEqual("{\"KEY\":\"VALUE\"}", TestDictionary.Serialize() );
        }

        /// <exclude />
        [TestMethod]
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

            Assert.AreEqual("{\"KEY\":\"VALUE\"}",  res);
        }

        /// <exclude />
        [TestMethod]
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

            Assert.AreEqual("{\"KEY\":\"VALUE\"}", res );
        }
    }
}
