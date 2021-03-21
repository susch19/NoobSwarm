
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class RandomColorEffect : LightEffect
    {
        private Random random = new();


        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            return Color.FromArgb(random.Next(0, Brightness + 1), random.Next(0, Brightness + 1), random.Next(0, Brightness + 1));
        }
    }
}
