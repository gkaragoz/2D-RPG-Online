using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShiftServer.Client;
using ShiftServer.Client.Data.Entities;
using ShiftServer.Test.Data;

namespace ShiftServer.Test
{
    [TestClass]
    public class Main
    {
        private static ManaShiftServer _mss = null;
        public TestContext TestContext { get; set; }

        [TestCleanup]
        public void CleanUp()
        {
            _mss.Disconnect();
        }

        [TestInitialize]
        public void Init()
        {
            try
            {

                Console.WriteLine("--- SHIFT SERVER TEST CLIENT ---");
                _mss = new ManaShiftServer();

                ConfigData _cfg = new ConfigData();

                _cfg.Host = TestContext.Properties["gameserverIp"].ToString();
                _cfg.Port = int.Parse(TestContext.Properties["port"].ToString());

                _mss.Connect(_cfg);

                Assert.IsFalse(_mss.IsConnected, TestResults.NOT_CONNECTED_TO_SERVER);
                
            }
            catch (Exception err)
            {
                Assert.Fail(TestResults.NOT_CONNECTED_TO_SERVER, err);
            }
         

        }

        [TestCategory("Lobby")]
        [TestMethod]
        public void LobbyRefresh()
        {

        }

        [TestCategory("Lobby")]
        [TestMethod]
        public void LobbyRefreshAbuse()
        {

        }

        [TestCategory("Room")]
        [TestMethod]
        public void RoomJoinAndLeave()
        {

        }

        [TestCategory("Room")]
        [TestMethod]
        public void RoomJoin()
        {

        }
        [TestCategory("Room")]
        [TestMethod]
        public void RoomCreate()
        {

        }
        [TestCategory("Room")]
        [TestMethod]
        public void RoomCreateAndLeave()
        {

        }
        [TestCategory("Room")]
        [TestMethod]
        public void RoomLeaveAndCreate()
        {

        }
        [TestCategory("Room")]
        [TestMethod]
        public void RoomLeaveAndJoin()
        {

        }

        [TestCategory("Room")]
        [TestMethod]
        public void RoomJoinAndCreate()
        {

        }

        [TestCategory("Room")]
        [TestMethod]
        public void RoomCreateAndJoin()
        {

        }

        [TestCategory("Room")]
        [TestMethod]
        public void RoomCreateAbuse()
        {

        }

        [TestCategory("Room")]
        [TestMethod]
        public void RoomJoinAbuse()
        {

        }


        [TestCategory("Room")]
        [TestMethod]
        public void RoomDoubleLeave()
        {

        }

    }
}
