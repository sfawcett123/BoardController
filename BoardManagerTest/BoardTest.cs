using Microsoft.VisualStudio.TestTools.UnitTesting;
using BoardManager;

namespace BoardManagerTests
{
    /// <exclude />
    [TestClass]
    public class BoardManagerTest
    {
        /// <exclude />
        [TestMethod, TestCategory("TCP")]
        public void TestBoard()
        {
            Board brd = new()
            {
               Name = "Test",
               OperatingSystem = OperatingSystems.ARDUINO.ToString(),
               Outputs = new() { "PLANE ALTITUDE" },
            };

            Assert.AreEqual("PLANE ALTITUDE", brd.Outputs[0]);

        }

        /// <exclude />
        [TestMethod, TestCategory("TCP")]
        public void TestValidateGood()
        {
            string result = Board.ValidateOS("ARDUINO");
            Assert.AreEqual(result, OperatingSystems.ARDUINO.ToString());
        }

        /// <exclude />
        [TestMethod, TestCategory("TCP")]
        public void TestValidateNull()
        {
            string result = Board.ValidateOS("");
            Assert.AreEqual(result, OperatingSystems.UNKNOWN.ToString());
        }

        /// <exclude />
        [TestMethod, TestCategory("TCP")]
        public void TestValidateBad()
        {
            string result = Board.ValidateOS("CHEESE");
            Assert.AreEqual(result, OperatingSystems.UNKNOWN.ToString());
        }

    }
}
