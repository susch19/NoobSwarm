using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class BreathingColorPerKeyEffect : PerKeyLightEffect
    {

        public Dictionary<LedKey, Color> KeysForBreathing { get; private set; }
        public PerKeyLightEffect? Effect { get; set; }
        private IReadOnlyList<LedKeyPoint>? keyPoints;

        public BreathingColorPerKeyEffect(Dictionary<LedKey, Color> keysForBreathing)
        {
            KeysForBreathing = keysForBreathing;
            LedKeys = KeysForBreathing.Keys.ToList();
        }

        public BreathingColorPerKeyEffect(List<LedKey> breathingKeys, PerKeyLightEffect effect)
        {
            LedKeys = breathingKeys;
            KeysForBreathing = new(breathingKeys.Count);
            foreach (var key in breathingKeys)
            {
                KeysForBreathing[key] = Color.White;
            }
            Effect = effect;
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            Initialized = true;
            keyPoints = ledKeyPoints;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            var step = counter % (255 * 2);
            bool bigger = step > 255;

            if (Effect is not null)
            {
                if (!Effect.Initialized && keyPoints is not null)
                    Effect.Init(keyPoints);
                else if (Effect.Initialized)
                    Effect.Next(KeysForBreathing, counter, elapsedMilliseconds, stepInrease, pressed);
            }

            if (bigger)
            {
                step -= 255;

                foreach (var keyCol in KeysForBreathing)
                {
                    if (!currentColors.ContainsKey(keyCol.Key))
                        continue;
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
                    if (!currentColors.ContainsKey(keyCol.Key))
                        continue;
                    var r = keyCol.Value.R * step / 255;
                    var g = keyCol.Value.G * step / 255;
                    var b = keyCol.Value.B * step / 255;
                    currentColors[keyCol.Key] = Color.FromArgb(keyCol.Value.A, keyCol.Value.R - r, keyCol.Value.G - g, keyCol.Value.B - b);
                }
            }

        }
    }
}
