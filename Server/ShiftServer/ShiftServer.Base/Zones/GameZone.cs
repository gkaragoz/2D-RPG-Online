using ShiftServer.Base.Auth;
using ShiftServer.Base.Core;
using ShiftServer.Base.Factory.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Telepathy;
using System.Threading;
using ShiftServer.Base.Zones;

namespace ShiftServer.Base.Worlds
{
    public class GameZone : Zone
    {
        private static readonly log4net.ILog log
             = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Vector3 Scale { get; set; }

        public GameZone()
        {

        }

        public void OnWorldUpdate()
        {
            IGameObject gObject = null;
            for (int i = 0; i < ObjectCounter; i++)
            {
                GameObjects.TryGetValue(i, out gObject);
                IGameInput gInput = null;
                PlayerInput pInput = null;
                for (int kk = 0; kk < gObject.GameInputs.Count; kk++)
                {
                    gObject.GameInputs.TryDequeue(out gInput);
                    //pInput = (PlayerInput)gInput;
                }
            }
        }
        public async System.Threading.Tasks.Task SendWorldStateAsync()
        {
            List<ShiftClient> clientList = Clients.GetValues();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].UserSessionID == null)
                    continue;

                ShiftServerData data = new ShiftServerData();

                clientList[i].SendPacket(MSPlayerEvent.WorldUpdate, data);
            }

        }
       
       
    }
}
