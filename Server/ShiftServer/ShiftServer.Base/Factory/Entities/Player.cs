using ShiftServer.Base.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace ShiftServer.Base.Factory.Entities
{
    public class Player : IGameObject
    {
        public Player()
        {
            GameInputs = new SafeQueue<IGameInput>();
            OwnedObjects = new List<IGameObject>();

        }
        public string Name { get; set; } = "Warrior";
        public int ObjectID { get; set; }
        public PlayerClass Class { get; set; }
        public int MaxHP { get; set; } = 100;
        public int CurrentHP { get; set; } = 100;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public int OwnerConnectionID { get; set; }
        public List<IGameObject> OwnedObjects { get; set; }
        public SafeQueue<IGameInput> GameInputs { get; set; }
        public string OwnerSessionID { get; set; }
        public double MovementSpeed { get; set; }
        public double AttackSpeed { get; set; }
        public int LastProcessedSequenceID { get; set; }

        public void OnAttack()
        {

        }

        public void OnHit()
        {

        }

        public void OnMove(Vector3 input)
        {
            this.Position += Vector3.Normalize(input) * (float)this.MovementSpeed * 0.02f;
        }
    }
}
