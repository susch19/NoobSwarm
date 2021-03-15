using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.Lights
{
    public enum KeyChangeState
    {
        None, 
        Pressed = 1<<1,
        Hold = 1 << 2,
        Release = 1 << 3
    }
}
