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

        public EntityState State { get; set; }

        public PlayerObject GetPlayerObject()
        {
            PlayerObject data = new PlayerObject
            {
                Oid = this.ObjectID,
                LastProcessedSequenceID = this.LastProcessedSequenceID
            };

            if (State == EntityState.NEWSPAWN)
            {
                data.PosX = this.Position.X;
                data.PosY = this.Position.Y;
                data.PosZ = this.Position.Z;
                data.RotX = this.Rotation.X;
                data.RotY = this.Rotation.Y;
                data.RotZ = this.Rotation.Z;
                data.AttackSpeed = (float)this.AttackSpeed;
                data.MovementSpeed = (float)this.MovementSpeed;
            }

            if (State == EntityState.MOVE)
            {
                data.PosX = this.Position.X;
                data.PosY = this.Position.Y;
                data.PosZ = this.Position.Z;
            }

            if (State == EntityState.GETHIT || State == EntityState.ATTACK)
            {
                data.CurrentHp = this.CurrentHP;
            }

            return data;       
        }


        public void OnAttack()
        {
            State = EntityState.ATTACK;
        }

        public void OnHit()
        {
            State = EntityState.GETHIT;
        }
        public void OnMove(Vector3 input)
        {
            if (State != EntityState.STUN)
            {
                input = Vector3.Normalize(input);

                Quaternion newRotation = GameEngine.QuaternionLookRotation(input, new Vector3(0, 1, 0));
                this.Rotation = new Vector3(newRotation.X, newRotation.Y, newRotation.Z);
                this.Position +=  input * (float)this.MovementSpeed * 0.02f;
                State = EntityState.MOVE;
            }
        }

        public void ResolveInputs()
        {
            IGameInput gInput = null;
            for (int kk = 0; kk <= this.GameInputs.Count; kk++)
            {
                this.GameInputs.TryDequeue(out gInput);
                if (gInput != null)
                {
                    switch (gInput.EventType)
                    {
                        case MSPlayerEvent.Move:
                            this.OnMove(gInput.Vector);
                            break;
                        case MSPlayerEvent.Attack:
                            break;
                        case MSPlayerEvent.Dead:
                            break;
                        case MSPlayerEvent.Use:
                            break;
                        default:
                            State = EntityState.IDLE;
                            break;
                    }

                    this.LastProcessedSequenceID = gInput.SequenceID;
                }
                //pInput = (PlayerInput)gInput;
            }
        }
    }
}
