using Google.Protobuf;
using ShiftServer.Client.Core;
using ShiftServer.Client.Data.Entities;
using ShiftServer.Proto.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ShiftServer.Client
{
    /// <summary>
    /// The main <c>network client</c> class.
    /// Contains all methods for performing game server functions.
    /// </summary>
    public sealed class ManaShiftServer
    {
        private static ManaShiftServer _mss = null;
        private static GameProvider _gameProvider = null;
        private static string _sessionID = null;
        public bool IsConnected { get => _gameProvider.IsClientConnected(); }
        public bool IsConnecting { get => _gameProvider.client.Connecting; }
        public bool HasPlayerRoom { get => _gameProvider.dataHandler.roomProvider.JoinedRoom == null ? false : true; }
        public MSSRoom JoinedRoom { get => _gameProvider.dataHandler.roomProvider.JoinedRoom; }
        public CommonAccountData AccountData { get => _gameProvider.dataHandler.accountData; }

        private Stopwatch _stopwatch;
        private long _currentPingValue;

        /// <summary>
        /// Constructor method of game client object
        /// </summary>
        public ManaShiftServer()
        {
            _mss = this;
            _gameProvider = new GameProvider();
            _stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Connect to tcp socket
        /// </summary>
        /// <param name="client">client object</param>
        /// <returns></returns>
        public IEnumerator Connect(ConfigData cfg, int timeout = 10)
        {
            if (string.IsNullOrEmpty(cfg.SessionID))
                throw new ArgumentNullException("Session ID is null");

            _sessionID = cfg.SessionID;
            _gameProvider.Connect(cfg.Host, cfg.Port);


            Stopwatch s = new Stopwatch();
            s.Start();

            while (this.IsConnected != true)
            {
                if (s.Elapsed > TimeSpan.FromSeconds(timeout))
                {
                    throw new TimeoutException("Connection timeout");
                }
                Console.WriteLine("connecting...");

                yield return null;
            };

            s.Stop();

            this.AddEventListener(MSServerEvent.PingRequest, this.OnPingResponse);
            this.JoinServer(_sessionID);
        }

        
        private void OnPingResponse(ShiftServerData data)
        {
            _stopwatch.Stop();
            _currentPingValue = _stopwatch.Elapsed.Milliseconds;
        }

        private void OnAccountJoinSuccess(ShiftServerData data)
        {

        }
        private void SendPingRequest()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            this.SendMessage(MSServerEvent.PingRequest);
        }

        /// <summary>
        /// Get Room List
        /// </summary>
        public List<Room> GetRoomList()
        {
            //sthis.SendMessage(MSServerEvent.LobbyRefresh);
            return _gameProvider.dataHandler.roomProvider.RoomList;
        }

        /// <summary>
        /// join server with session id provided from auth server
        /// </summary>
        private void JoinServer(string sessionID)
        {
            ShiftServerData data = new ShiftServerData();
            data.SessionID = sessionID;            
            this.SendMessage(MSServerEvent.AccountJoin, data);
        }
        /// <summary>
        /// Fixed Update
        /// </summary>
        /// <param name="client">client object</param>
        /// <returns></returns>
        public void Setup()
        {
            _gameProvider.SetupTasks();
        }

        /// <summary>
        /// Add event handler function with giving message enum
        /// </summary>
        /// <param name="eventType">Shift Server event list</param>
        /// <param name="listener">Callback function which fired when event triggered</param>
        public void AddEventListener(object eventType, Action<ShiftServerData> listener)
        {
            if (eventType.GetType() == typeof(MSServerEvent))
            {
                _gameProvider.dataHandler.clientEvents.Add(new ClientEventCallback
                {
                    CallbackFunc = listener,
                    EventId = (MSServerEvent)eventType
                });
            }
            else if (eventType.GetType() == typeof(MSPlayerEvent))
            {
                _gameProvider.dataHandler.playerEvents.Add(new PlayerEventCallback
                {
                    CallbackFunc = listener,
                    EventId = (MSPlayerEvent)eventType
                });
            }
          
        }
        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            _gameProvider.Disconnect();
        }

      

        /// <summary>
        /// Craft and send data to server
        /// </summary>
        public void SendMessage(MSServerEvent evt, ShiftServerData data = null)
        {
            if (data == null)
                data = new ShiftServerData();

            data.Basevtid = MSBaseEventId.ServerEvent;
            data.Svevtid = evt;
            data.SessionID = _sessionID;

            byte[] bb = data.ToByteArray();

            if (bb.Length > 0)
                _gameProvider.SendMessage(bb);
        }

       
        /// <summary>
        /// Craft and send data to server
        /// </summary>
        public void SendMessage(MSPlayerEvent evt, ShiftServerData data)
        {
            if (data == null)
                throw new Exception();

            data.Basevtid = MSBaseEventId.PlayerEvent;
            data.Plevtid = evt;
            data.SessionID = _sessionID;

            byte[] bb = data.ToByteArray();

            if (bb.Length > 0)
                _gameProvider.SendMessage(bb);
        }


    }
}
