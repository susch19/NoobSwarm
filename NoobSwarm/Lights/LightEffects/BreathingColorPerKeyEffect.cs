using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class BreathingColorPerKeyEffect : LightEffect
    {
        public Dictionary<LedKey, Color> KeysForBreathing { get; private set; }
        
        public BreathingColorPerKeyEffect(Dictionary<LedKey, Color> keysForBreathing)
        {
            KeysForBreathing = keysForBreathing; 
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            var step = counter % (255 * 2);
            bool bigger = step > 255;


            if (bigger)
            {
                step -= 255;
            }
          
            if (bigger)
            {

                foreach (var keyCol in KeysForBreathing)
                {
                    var r = keyCol.Value.R * step / 255;
                    var g = keyCol.Value.G * step / 255;
                    var b = keyCol.Value.B * step / 255;
                    currentColors[keyCol.Key] = Color.FromArgb(keyCol.Value.A, r, g, b);
                }
            }
            else
            {
                foreach (var keyCol in KeysForBreathing)
                {
                    var r = keyCol.Value.R * step / 255;
                    var g = keyCol.Value.G * step / 255;
                    var b = keyCol.Value.B * step / 255;
                    currentColors[keyCol.Key] = Color.FromArgb(keyCol.Value.A, keyCol.Value.R - r, keyCol.Value.G - g, keyCol.Value.B - b);
                }

            }

        }
    }
}
