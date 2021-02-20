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
        public float Speed { get => speed; set { if (value == speed) return; speed = Math.Max(value, 0f); } }
        public byte Brightness { get; set; } = 255;
        protected IReadOnlyList<LedKeyPoint>? LedKeyPoints { get; set; }

        protected float BrightnessPercent => Brightness / 255f;


        private float speed = 1f;

        public abstract void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed);

        public virtual void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            this.LedKeyPoints = ledKeyPoints;
            Initialized = true;
        }

        public virtual void Info(int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyCollection<LedKey> pressed) { }
    }
    public abstract class PerKeyLightEffect : LightEffect
    {
        public List<LedKey>? LedKeys { get; set; }
    }
}