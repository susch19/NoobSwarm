using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class PressedFadeOutEffect : LightEffect
    {
        public bool FasterPreKeyPress { get; set; }

        private Dictionary<LedKey, (short value, byte multiplier)> keyFades = new();
        private Color color;
        private byte biggest;

        public PressedFadeOutEffect(Color c, bool fasterPreKeyPress = false)
        {
            color = c;
            FasterPreKeyPress = fasterPreKeyPress;
            biggest = Math.Max(Math.Max(color.R, color.G), color.B);
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            UpdateKeyPressed(pressed);

            if (keyFades.Count > 0)
            {
                Dictionary<LedKey, short>? toDelete = null;
                var step = (byte)Math.Min((stepInrease * Speed), 255);

                foreach (var fade in keyFades)
                {
                    if (FasterPreKeyPress)
                        step = (byte)Math.Min((stepInrease * Speed) + fade.Value.multiplier, 255);

                    var r = (byte)((color.R * fade.Value.value / 255) * brightnessPercent);
                    var g = (byte)((color.G * fade.Value.value / 255) * brightnessPercent);
                    var b = (byte)((color.B * fade.Value.value / 255) * brightnessPercent);

                    currentColors[fade.Key] = Color.FromArgb(color.A, r, g, b);
                    keyFades[fade.Key] = ((short)(keyFades[fade.Key].value - step), 0);

                    if (keyFades[fade.Key].value <= 0)
                    {
                        toDelete ??= new();
                        toDelete.Add(fade.Key, fade.Value.value);
                    }
                }

                if (toDelete is not null)
                {
                    foreach (var key in toDelete)
                        keyFades.Remove(key.Key);
                }
            }
        }
        public override void Info(int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyCollection<LedKey> pressed)
        {
            UpdateKeyPressed(pressed);
        }

        private void UpdateKeyPressed(IReadOnlyCollection<LedKey> pressed)
        {
            foreach (var press in pressed)
            {
                keyFades[press] = (biggest, 0);
                if (FasterPreKeyPress)
                {
                    for (int i = 0; i < keyFades.Count; i++)
                    {
                        var other = keyFades.ElementAt(i);
                        if (other.Key == press)
                            continue;

                        keyFades[other.Key] = (other.Value.value, (byte)Math.Min(255, other.Value.multiplier + 10));
                    }
                }
            }
        }
    }
}
