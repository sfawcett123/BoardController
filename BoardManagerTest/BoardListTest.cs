﻿using BoardManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;


namespace BoardManagerTests
{
    /// <exclude />
    [TestClass]
    public class BoardListTest
    {
        /// <summary>
        /// Dummy wrapper class for Abstract BoardList
        /// </summary>
        /// <seealso cref="BoardList" />
        internal class BoardTest : BoardList
        {
        }
        private readonly Board testBoard1 = new()
        {
            Name = "Test1",
            OperatingSystem = OperatingSystems.ARDUINO.ToString(),
            Outputs = new() { "PLANE ALTITUDE" },

        };

        private readonly Board testBoard2 = new()
        {
            Name = "Test2",
            OperatingSystem = OperatingSystems.ARDUINO.ToString(),
            Outputs = new() { "PLANE ALTITUDE" },

        };



        /// <exclude />
        [TestMethod]
        public void TestAddBoard()
        {
            BoardTest bd = new();
            string result = bd.Add(testBoard1);

            Assert.IsNotNull(result);
            Dictionary<string, string>? data = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

            Assert.IsNotNull(data);
            Assert.AreEqual(testBoard1.Name, data["name"]);
            Assert.AreEqual(testBoard1.OperatingSystem, data["os"]);
        }

        /// <exclude />
        [TestMethod]
        public void TestInternalTimeOut()
        {
            BoardTest bd = new();
            string result = bd.Add(testBoard1);

            Assert.IsNotNull(result);

            List<BoardDetails> list = bd.GetBoards();

            Assert.AreEqual(1, list.Count);

            list[0].Timeout = 9999;
            list[0].BoardInternal = true;

            bd.RemoveTimedOut(1);

            Assert.AreEqual(1, list.Count);
        }

        /// <exclude />
        [TestMethod]
        public void TestExternalTimeOut()
        {
            BoardTest bd = new();
            string result = bd.Add(testBoard1);
            bd.Add(testBoard2);

            Assert.IsNotNull(result);

            List<BoardDetails> list = bd.GetBoards();

            Assert.AreEqual(2, list.Count);

            list[0].Timeout = 9999; // Force timeout of first board
            list[0].BoardInternal = false;

            list[1].Timeout = 0;    // Ensure second board isnt timed out
            list[1].BoardInternal = false;

            bd.RemoveTimedOut(100);

            Assert.AreEqual(1, list.Count);
        }

        /// <exclude />
        [TestMethod]
        public void TestOutputData()
        {
            BoardTest bd = new();
            string result1 = bd.Add(testBoard1);
            Assert.IsNotNull(result1);
            Dictionary<string, string> dic = new()
            {
                { "Test", "Data" }
            };

            bd.SetOutputData(dic);
            Dictionary<string, string> dic2 = bd.GetAllOutputData();

            Assert.AreEqual(dic.Serialize(), dic2.Serialize());
        }
    }
}
