using System;
using System.Collections.Generic;
using System.Drawing;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public abstract class LightEffect
    {
        public bool Initialized { get; internal set; }

        public abstract void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, IReadOnlyList<LedKey> pressed);

        public abstract void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints);
    }
}