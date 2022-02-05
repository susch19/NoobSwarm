using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class SolidColorEffect : LightEffect
    {
        public Color? SolidColor { get; set; }
        public SolidColorEffect()
        {
        }
        public SolidColorEffect(Color? color)
        {
            SolidColor = color;
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            if (!SolidColor.HasValue)
                return null;

            return GetColorWithBrightness(SolidColor.Value);
        }
    }
}
