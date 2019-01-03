﻿using ShiftServer.Server.Auth;
using ShiftServer.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Server.Groups
{
    public class WorldGroup : IGroup
    {
        public SafeDictionary<int, IGameObject> GameObjects { get; set; }

        public void OnAccept(ShiftClient client, IGameObject gameObject)
        {
        }

        public void OnInvite(ShiftClient client, IGameObject gameObject)
        {

        }

        public void OnKick(ShiftClient client, IGameObject gameObject)
        {
        }
    }
}