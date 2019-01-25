using PhysX;
using System.Collections.Generic;
using System.Numerics;
using Telepathy;

namespace ShiftServer.Base.Core
{
    public interface IGameObject
    {
        string Name { get; set; }
        int OwnerConnectionID { get; set; }
        string OwnerSessionID { get; set; }
        int ObjectID { get; set; }
        PlayerClass Class { get; set; }
        List<IGameObject> OwnedObjects { get; set; }
        SafeQueue<IGameInput> GameInputs { get; set; }
        RigidDynamic RigidDynamic { get; set; }
        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }
        Vector3 Scale { get; set; }
        double MovementSpeed { get; set; }
        double AttackSpeed { get; set; }
        double AttackRange { get; set; }
        int AttackDamage { get; set; }
        int CurrentHP { get; set; }
        int MaxHP { get; set; }
        int MaxMana { get; set; }
        int CurrentMana { get; set; }
        int Strenght { get; set; }
        int Dexterity { get; set; }
        int Intelligence { get; set; }
        int LastProcessedSequenceID { get; set; }
        void OnAttack();
        void OnHit();
        void OnMove(IGameInput input);
        void ResolveInputs();
        void ResolveInputs(IGameInput input);
        PlayerObject GetPlayerObject();
    }
}
