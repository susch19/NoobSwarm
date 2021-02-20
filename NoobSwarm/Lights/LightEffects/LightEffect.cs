using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public abstract class LightEffect
    {
        public bool Initialized { get; internal set; }
        public bool Active { get; set; } = true;

        public abstract void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed);

        public abstract void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints);

        public virtual void Info(int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyCollection<LedKey> pressed) { }
    }
    public abstract class PerKeyLightEffect : LightEffect
    {
        public List<LedKey>? LedKeys { get; set; }
    }
}