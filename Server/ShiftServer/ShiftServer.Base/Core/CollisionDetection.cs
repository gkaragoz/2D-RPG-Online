using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Base.Core
{
    public class CollisionDetection
    {
        // collision configuration contains default setup for memory, collision setup
        DefaultCollisionConfiguration CollisionConf = new DefaultCollisionConfiguration();
        CollisionDispatcher Dispatcher = null;
        public CollisionDetection()
        {
            Dispatcher = new CollisionDispatcher(CollisionConf);
        }
        public static void CollisionTest()
        {
        }
    }
}
