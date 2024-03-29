﻿

using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class InverseKeysColorEffect : LightEffect
    {
        public InverseKeysColorEffect()
        {
            Initialized = true;
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            return GetColorWithBrightness(Color.FromArgb((byte)((currentColor.R ^ 0xff)), (byte)((currentColor.G ^ 0xff)), (byte)((currentColor.B ^ 0xff))));
        }
    }
}
