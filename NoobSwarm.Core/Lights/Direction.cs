using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.Lights
{
    public enum Direction
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Up = 1 << 2,
        Down = 1 << 3,
        //UpLeft,
        //UpRight,
        //DownLeft,
        //DownRight,
    }
}
