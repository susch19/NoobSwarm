

using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class InverseKeysColorEffect : LightEffect
    {
        public List<LedKey>? Keys { get; set; }

        public InverseKeysColorEffect()
        {
            Initialized = true;
        }
        public InverseKeysColorEffect(List<LedKey> keys)
        {
            Keys = keys;
            Initialized = true;
        }


        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (Keys is not null)
                foreach (var key in Keys)
                {
                    if (currentColors.TryGetValue(key, out var curColor))
                        currentColors[key] = Color.FromArgb((byte)((curColor.R ^ 0xff) * BrightnessPercent), (byte)((curColor.G ^ 0xff) * BrightnessPercent), (byte)((curColor.B ^ 0xff) * BrightnessPercent));
                }
            else
            {
                foreach (var key in currentColors)
                {
                    currentColors[key.Key] = Color.FromArgb((byte)((key.Value.R ^ 0xff) * BrightnessPercent), (byte)((key.Value.G ^ 0xff) * BrightnessPercent), (byte)((key.Value.B ^ 0xff) * BrightnessPercent));
                }
            }
        }
    }
}
