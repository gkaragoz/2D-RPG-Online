﻿using ShiftServer.Server.Auth;
using ShiftServer.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Rooms
{
    public class BattlegroundRoom : IRoom
    {
        public SafeDictionary<int, IGameObject> GameObjects { get; set; }
        public SafeDictionary<int, ShiftClient> Clients { get; set; }
        public SafeDictionary<string, int> SocketIdSessionLookup { get; set; }
        public int MaxUser { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public BattlegroundRoom()
        {
            Clients = new SafeDictionary<int, ShiftClient>();
            SocketIdSessionLookup = new SafeDictionary<string, int>();
            GameObjects = new SafeDictionary<int, IGameObject>();
        }
        public void OnGameStart(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnObjectAttack(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnObjectCreate(IGameObject gameObject)
        {
        }

        public void OnObjectDestroy(IGameObject gameObject)
        {
        }

        public void OnObjectMove(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnObjectUse(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnPlayerCreate(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnPlayerJoin(ShiftServerData data, ShiftClient client)
        {
        }

        public void OnRoomUpdate()
        {
        }

        public void SendRoomState()
        {
        }
    }
}
