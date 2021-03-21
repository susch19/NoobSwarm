
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public abstract class LightEffect
    {
        protected event EventHandler<bool>? Activated;

        [JsonIgnore]
        public bool Initialized { get; internal set; }
        public bool Active
        {
            get => active; set
            {
                active = value;
                Activated?.Invoke(this, value);
            }
        }
        public float Speed { get => speed; set { if (value == speed) return; speed = Math.Max(value, 0f); } }

        public byte Brightness { get; set; } = 255;
        protected IReadOnlyList<LedKeyPoint>? LedKeyPoints { get; set; }

        protected float BrightnessPercent => Brightness / 255f;


        private float speed = 1f;
        private bool active = true;

        /// <summary>
        /// Gets called per key to calculate the color to be displayed
        /// </summary>
        /// <param name="key">The key that shows the color</param>
        /// <param name="currentColor">The color that the key currently has</param>
        /// <param name="counter">The Update Iteration</param>
        /// <param name="elapsedMilliseconds">Elapsed time since start</param>
        /// <param name="stepInrease">The amount the counter should be increads</param>
        /// <returns></returns>
        public abstract Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease);

        /// <summary>
        /// Used to do one time calculation for the effect. Should only be called once per Update
        /// </summary>
        /// <param name="counter">The Update Iteration</param>
        /// <param name="elapsedMilliseconds">Elapsed time since start</param>
        /// <param name="stepInrease">The amount the counter should be increads</param>
        /// <param name="pressed">A list of keys and how they changed since the last update</param>
        /// <returns></returns>
        public virtual bool InitNextFrame(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            return true;
        }

        public virtual void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            this.LedKeyPoints = ledKeyPoints;
            Initialized = true;
        }

        public virtual void Info(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed) { }

        protected Color GetColorWithBrightness(Color color)
        {
            return Color.FromArgb(color.A, (byte)(color.R * BrightnessPercent), (byte)(color.G * BrightnessPercent), (byte)(color.B * BrightnessPercent));
        }
    }

}