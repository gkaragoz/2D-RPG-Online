using PhysX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Core
{
    public class PhysxEngine
    {
        private static PhysX.Foundation _foundation;
        private static PhysX.Physics _physics;

        public PhysxEngine()
        {

        }

        private Physics CreatePhysics(ErrorCallback errorCallback = null)
        {
            if (Physics.Instantiated)
                ServerProvider.log.Fatal("[PHYSX] > Physics has been already instantiated");


            if (_physics != null)
            {
                _physics.Dispose();
                _physics = null;
            }
            if (_foundation != null)
            {
                _foundation.Dispose();
                _foundation = null;
            }

            _foundation = new Foundation(errorCallback);
            _physics = new Physics(_foundation, checkRuntimeFiles: true);
            ServerProvider.log.Info("[PHYSX] > Physics instantiated successfully");

            return _physics;
        }
    }
}
