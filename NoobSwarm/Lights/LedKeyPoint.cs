using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights
{
    public class LedKeyPoint
    {
        public LedKeyPoint(int x, int y, LedKey ledKey)
        {
            X = x;
            Y = y;
            LedKey = ledKey;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public LedKey LedKey { get; set; }

    }
}
