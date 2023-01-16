using BoardManager;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;
using System.Net;
using System.Xml.Serialization;

namespace BoardManagerTest
{
    [TestClass]
    public class TestBoardDetails
    {

        private readonly BoardDetails bd = new BoardDetails(false)
        {
            Name = "TEST",
            BoardInternal = true,
            Rate = 1,
            OS = OperatingSystems.ARDUINO.ToString(),
        };

        private readonly BoardDetails bd2 = new BoardDetails(false)
        {
            Name = "TEST2",
            BoardInternal = true,
            Rate = 1,
            OS = OperatingSystems.ARDUINO.ToString(),
        };

        [TestMethod, TestCategory("TCP")]
        public void TestToString()
        {
            Assert.IsNotNull(bd);

            string result = bd.ToString();
            
            Assert.AreEqual(result,"Unknown" );
        }

        [TestMethod]
        public void StartBoard()
        {          
            bd.Start();
            Assert.IsTrue(bd.TimeStarted);
            Assert.AreNotEqual(bd.IPAddress, "Unknown");
        }

        [TestMethod]
        public void Compare()
        {
            Assert.AreNotEqual(bd, bd2);
            if (bd2 == bd) Assert.Fail("Boards should not be equal");
 
        }

    }
}
