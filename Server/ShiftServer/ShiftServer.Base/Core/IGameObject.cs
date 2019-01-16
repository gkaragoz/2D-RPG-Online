﻿using System.Collections.Generic;
using System.Numerics;
using Telepathy;

namespace ShiftServer.Base.Core
{
    public interface IGameObject
    {
        string Name { get; set; }
        int OwnerConnectionId { get; set; }
        string OwnerSessionId { get; set; }
        int ObjectId { get; set; }
        List<IGameObject> OwnedObjects { get; set; }
        SafeQueue<IGameInput> GameInputs { get; set; }
        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }
        Vector3 Scale { get; set; }
        double MovementSpeed { get; set; }
        double AttackSpeed { get; set; }
        int CurrentHP { get; set; }
        int MaxHP { get; set; }
        void OnAttack();
        void OnHit();
        void OnMove(Vector3 rotation);

    }
}
