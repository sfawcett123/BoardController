﻿using BoardManager.boardManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using System.Text.Json;


namespace BoardManager.boardManagerTests
{
    /// <exclude />
    [TestClass]
    public class BoardListTest
    {
        /// <summary>
        /// Dummy wrapper class for Abstract BoardList
        /// </summary>
        /// <seealso cref="BoardManager.boardManager.BoardList" />
        internal class boardTest : BoardList
        {
        }

        private Board testBoard1 = new Board()
        {
            Name = "Test",
            OperatingSystem = OperatingSystems.ARDUINO.ToString(),
            Outputs = new() { "PLANE ALTITUDE" },

        };

        /// <exclude />
        [TestMethod]
        public void TestAddBoard()
        {
            boardTest bd = new boardTest();
            string result = bd.Add(testBoard1);

            Assert.IsNotNull(result);
            Dictionary<string,string> data = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

            Assert.IsNotNull(data);
            Assert.AreEqual( testBoard1.Name           , data["name"] );
            Assert.AreEqual( testBoard1.OperatingSystem, data["os"]);
        }

        /// <exclude />
        [TestMethod]
        public void TestInternalTimeOut()
        {
            boardTest bd = new boardTest();
            string result = bd.Add(testBoard1);

            Assert.IsNotNull(result);

            List<BoardDetails> list = bd.GetBoards();

            Assert.AreEqual( 1 , list.Count );

            list[0].Timeout = 9999;
            list[0].BoardInternal = true;

            bd.RemoveTimedOut( 1 );

            Assert.AreEqual( 1 , list.Count );
        }

        /// <exclude />
        [TestMethod]
        public void TestExternalTimeOut()
        {
            boardTest bd = new boardTest();
            string result = bd.Add(testBoard1);

            Assert.IsNotNull(result);

            List<BoardDetails> list = bd.GetBoards();

            Assert.AreEqual(1, list.Count);

            list[0].Timeout = 9999;
            list[0].BoardInternal = false;

            bd.RemoveTimedOut(1);

            Assert.AreEqual(0, list.Count);
        }

        /// <exclude />
        [TestMethod]
        public void TestOutputData()
        {
            boardTest bd = new boardTest();
            string result1 = bd.Add(testBoard1);
            Assert.IsNotNull(result1);
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "Test", "Data" }
            };

            bd.SetOutputData(dic);
            Dictionary<string, string> dic2 = bd.GetAllOutputData();

            Assert.AreEqual(dic.Serialize(), dic2.Serialize());
        }
    }
}
