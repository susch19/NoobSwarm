using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class BreathingColorEffect : PerKeyLightEffect
    {
        public Color Color
        {
            get => color; set
            {
                if (color == value) return;
                color = value;
                biggest = Math.Max(Math.Max(Color.R, Color.G), Color.B);
            }
        }

        private byte biggest;
        private Color color;

        public BreathingColorEffect(Color color)
        {
            LedKeys = null;
            Color = color;
        }
        public BreathingColorEffect(List<LedKey> breathingKeys, Color color)
        {
            LedKeys = breathingKeys;
            Color = color;
        }



        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            
            var step = (int)(counter*Speed) % (biggest * 2);
            bool bigger = step > biggest;

            if (bigger)
            {
                step -= biggest;
            }

            var r = Color.R * step / biggest;
            var g = Color.G * step / biggest;
            var b = Color.B * step / biggest;


            if (bigger)
            {

                foreach (var key in (LedKeys is null ? currentColors.Keys : (IReadOnlyCollection<LedKey>)LedKeys))
                {
                    currentColors[key] = Color.FromArgb(Color.A, r, g, b);
                }
            }
            else
            {
                foreach (var key in (LedKeys is null ? currentColors.Keys : (IReadOnlyCollection<LedKey>)LedKeys))
                {
                    currentColors[key] = Color.FromArgb(Color.A, Color.R - r, Color.G - g, Color.B - b);
                }

            }

        }
    }
}
