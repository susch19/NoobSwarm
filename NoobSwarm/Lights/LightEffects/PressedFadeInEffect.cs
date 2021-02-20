using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class PressedFadeInEffect : PerKeyLightEffect
    {

        private Dictionary<LedKey, short> keyFades = new();
        private Color color;
        private PerKeyLightEffect? effect;
        private byte biggest;

        public PressedFadeInEffect(Color c)
        {
            color = c;
            biggest = Math.Max(Math.Max(color.R, color.G), color.B);
        }

        public PressedFadeInEffect(PerKeyLightEffect e)
        {
            effect = e;
            biggest = 255;
        }

        public PressedFadeInEffect(Color c, List<LedKey> ledKeys)
        {
            color = c;
            biggest = Math.Max(Math.Max(color.R, color.G), color.B);
            LedKeys = ledKeys;
        }

        public PressedFadeInEffect(PerKeyLightEffect e, List<LedKey> ledKeys)
        {
            effect = e;
            LedKeys = ledKeys;
            biggest = 255;
        }


        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            foreach (var press in pressed)
            {
                keyFades[press] = 0;
            }
            if (keyFades.Count > 0)
            {
                var step = (byte)Math.Min((stepInrease * Speed), 255);

                Dictionary<LedKey, Color>? effectColors = null;
                Dictionary<LedKey, short>? toDelete = null;

                if (effect is not null)
                {
                    effectColors = keyFades.ToDictionary(x => x.Key, x => Color.Black);
                    if (!effect.Initialized && LedKeyPoints is not null)
                        effect.Init(LedKeyPoints);
                    else if (effect.Initialized)
                        effect.Next(effectColors, counter, elapsedMilliseconds, stepInrease, pressed);
                }

                foreach (var fade in keyFades)
                {
                    Color localColor;
                    if (effectColors is null || !effectColors.TryGetValue(fade.Key, out localColor))
                    {
                        localColor = color;
                    }

                    var r = localColor.R * fade.Value / 255;
                    var g = localColor.G * fade.Value / 255;
                    var b = localColor.B * fade.Value / 255;
                    currentColors[fade.Key] = Color.FromArgb(localColor.A, (byte)(r * BrightnessPercent), (byte)(g * BrightnessPercent), (byte)(b * BrightnessPercent));
                    keyFades[fade.Key] += step;
                    if (keyFades[fade.Key] > biggest)
                    {
                        if (toDelete is null)
                            toDelete = new();
                        toDelete.Add(fade.Key, fade.Value);
                    }
                }

                if (toDelete is not null)
                    foreach (var key in toDelete)
                    {
                        keyFades.Remove(key.Key);
                    }
            }
        }

        public override void Info(int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyCollection<LedKey> pressed)
        {
            foreach (var press in pressed)
            {
                keyFades[press] = 0;
            }
        }
    }
}
