using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.Makros
{
    [Flags]
    public enum KeyModifier : byte
    {
        None = 0,
        Left_Control = 1 << 0,
        Left_Shift = 1 << 1,
        Left_Alt = 1 << 2,
        Left_Gui = 1 << 3,
        Right_Control = 1 << 4,
        Right_Shift = 1 << 5,
        Right_Alt = 1 << 6,
        Right_Gui = 1 << 7
    }
}
