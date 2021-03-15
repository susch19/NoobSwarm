
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class BreathingEffectEffect : LightEffect
    {

        public LightEffect? Effect { get; set; }

  
        public BreathingEffectEffect(LightEffect effect)
        {
          
            Effect = effect;
        }


        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> KeyChangeState)
        {
            var step = (int)(counter * Speed) % (255 * 2);
            bool bigger = step > 255;

            if (Effect is not null)
            {
                if (!Effect.Initialized && LedKeyPoints is not null)
                    Effect.Init(LedKeyPoints);
                else if (Effect.Initialized)
                    Effect.Next(currentColors, counter, elapsedMilliseconds, stepInrease, KeyChangeState);
            }

            if (bigger)
            {
                step -= 255;

                foreach (var keyCol in currentColors)
                {
                    if (!currentColors.ContainsKey(keyCol.Key))
                        continue;
                    var r = (byte)((keyCol.Value.R * step / 255) * BrightnessPercent);
                    var g = (byte)((keyCol.Value.G * step / 255) * BrightnessPercent);
                    var b = (byte)((keyCol.Value.B * step / 255) * BrightnessPercent);
                    currentColors[keyCol.Key] = Color.FromArgb(keyCol.Value.A, r, g, b);
                }
            }
            else
            {
                foreach (var keyCol in currentColors)
                {
                    if (!currentColors.ContainsKey(keyCol.Key))
                        continue;
                 
                    var r = (byte)((keyCol.Value.R - (keyCol.Value.R * step / 255)) * BrightnessPercent);
                    var g = (byte)((keyCol.Value.G - (keyCol.Value.G * step / 255)) * BrightnessPercent);
                    var b = (byte)((keyCol.Value.B - (keyCol.Value.B * step / 255)) * BrightnessPercent);
                    currentColors[keyCol.Key] = Color.FromArgb(keyCol.Value.A, r, g, b);
                }
            }

        }
    }
}
