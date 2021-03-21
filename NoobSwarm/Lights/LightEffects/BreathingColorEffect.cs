using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class BreathingColorEffect : LightEffect
    {
      
        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            byte biggest;
            if (currentColor.R > currentColor.G && currentColor.R > currentColor.B)
                biggest = currentColor.R;
            else if (currentColor.G > currentColor.R && currentColor.G > currentColor.B)
                biggest = currentColor.G;
            else
                biggest = currentColor.B;

            var step = (int)(counter * Speed) % (biggest * 2);
            var bigger = step > biggest;

            if (bigger)
            {
                step -= biggest;
            }

            var r = currentColor.R * step / biggest;
            var g = currentColor.G * step / biggest;
            var b = currentColor.B * step / biggest;

            if (bigger)
            {
                return GetColorWithBrightness(Color.FromArgb(currentColor.A, r, g, b));
            }
            else
            {
                var rDown = (byte)((currentColor.R - r));
                var gDown = (byte)((currentColor.G - g));
                var bDown = (byte)((currentColor.B - b));
                return GetColorWithBrightness(Color.FromArgb(currentColor.A, rDown, gDown, bDown));
            }
        }
    }
}
