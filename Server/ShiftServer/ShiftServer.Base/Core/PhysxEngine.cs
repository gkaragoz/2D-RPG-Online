using PhysX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Core
{
    public class PhysxEngine
    {
        public static PhysxEngine Engine;
        private static PhysX.Physics Core;
        private static PhysX.Foundation _foundation;

        public PhysxEngine()
        {
            Engine = this;
            this.CreatePhysics();
        }
        public RigidDynamic CreateRigidDynamic(Scene scene)
        {
            var material = scene.Physics.CreateMaterial(0.7f, 0.7f, 0.1f);
            

            RigidDynamic rigidActor = scene.Physics.CreateRigidDynamic();
            //to-do gokhanla incele
            var boxGeom = new BoxGeometry(2, 2, 2);
            var boxShape = rigidActor.CreateShape(boxGeom, material);

            rigidActor.GlobalPose = Matrix4x4.CreateTranslation(-20, 10  * (boxGeom.Size.Y + 0.5f), 0);
            rigidActor.SetMassAndUpdateInertia(10);

            scene.AddActor(rigidActor);

            return rigidActor;

        }
        public Scene CreateScene()
        {
            return Core.CreateScene();
        }
        private Physics CreatePhysics(ErrorCallback errorCallback = null)
        {
            if (Physics.Instantiated)
                ServerProvider.log.Fatal("[PHYSX] > Physics has been already instantiated");


            if (Core != null)
            {
                Core.Dispose();
                Core = null;
            }
            if (_foundation != null)
            {
                _foundation.Dispose();
                _foundation = null;
            }

            _foundation = new Foundation(errorCallback);
            Core = new Physics(_foundation, checkRuntimeFiles: true);
            ServerProvider.log.Info("[PHYSX] > Physics instantiated successfully");

            return Core;
        }
    }
}
