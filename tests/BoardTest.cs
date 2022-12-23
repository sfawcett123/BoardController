using BoardManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoardManagerTests
{
    /// <exclude />
    [TestClass]
    public class BoardManagerTest
    {
        /// <exclude />
        [TestMethod]
        public void TestBoard()
        {
            BoardManager.Board brd = new()
            {
                Name = "Test",
                OperatingSystem = BoardManager.OperatingSystems.ARDUINO.ToString(),
                Outputs = new() { "PLANE ALTITUDE" },
            };

            Assert.AreEqual("PLANE ALTITUDE", brd.Outputs[0] );

        }

        /// <exclude />
        [TestMethod]
        public void TestValidateGood()
        {
            string result = BoardManager.Board.ValidateOS("ARDUINO");
            Assert.AreEqual(result, OperatingSystems.ARDUINO.ToString() );
        }

        /// <exclude />
        [TestMethod]
        public void TestValidateNull()
        {
            string result = BoardManager.Board.ValidateOS("");
            Assert.AreEqual(result, OperatingSystems.UNKNOWN.ToString());
        }

        /// <exclude />
        [TestMethod]
        public void TestValidateBad()
        {
            string result = BoardManager.Board.ValidateOS("CHEESE");
            Assert.AreEqual(result, OperatingSystems.UNKNOWN.ToString());
        }

    }
}
