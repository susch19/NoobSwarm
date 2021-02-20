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
    public class PressedFadeInEffect : LightEffect
    {

        private Dictionary<LedKey, short> keyFades = new();
        private Color color;
        private byte biggest;
        public PressedFadeInEffect(Color c)
        {
            color = c;

            biggest = Math.Max(Math.Max(color.R, color.G), color.B);
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            foreach (var press in pressed)
            {
                keyFades[press] = biggest;
            }
            if (keyFades.Count > 0)
            {
                var step = (byte)Math.Min((stepInrease * Speed), 255);

                Dictionary<LedKey, short>? toDelete = null;

                foreach (var fade in keyFades)
                {
                    var r = color.R * fade.Value / 255;
                    var g = color.G * fade.Value / 255;
                    var b = color.B * fade.Value / 255;
                    currentColors[fade.Key] = Color.FromArgb(color.A, (byte)((color.R - r)*brightnessPercent), (byte)((color.G - g) * brightnessPercent), (byte)((color.B - b)*brightnessPercent));
                    keyFades[fade.Key] += step;
                    if (keyFades[fade.Key] > 255)
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
                keyFades[press] = biggest;
            }
        }
    }
}
