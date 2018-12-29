using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftServer.Server.Core
{
    interface IGameObject
    {
        void OnAttack();
        void OnHit();
        void OnMove(); 

    }
}
