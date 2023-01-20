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

        [TestMethod]
        public void StartBoard()
        {          
            bd.Start();
            Assert.AreEqual("Unknown", bd.ConnectedAddress);
        }

        [TestMethod]
        public void Compare()
        {
            Assert.AreNotEqual(bd, bd2);
            if (bd2 == bd) Assert.Fail("Boards should not be equal");
 
        }

        [TestMethod]
        public void TestTimeOut()
        {
            bd.Start();
            Assert.AreEqual(0 , bd.Timeout );
            Thread.Sleep(3000);
            Assert.AreNotEqual(0, bd.Timeout);
        }

    }
}
