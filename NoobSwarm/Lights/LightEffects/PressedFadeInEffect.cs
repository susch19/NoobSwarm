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

        private Dictionary<LedKey, byte> keyFades = new();
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

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, IReadOnlyList<LedKey> pressed)
        {
            foreach (var press in pressed)
            {
                keyFades[press] = biggest;
            }
            if (keyFades.Count > 0)
            {

                Dictionary<LedKey, byte> toDelete = new();

                foreach (var fade in keyFades)
                {
                    var r = color.R * fade.Value / 255;
                    var g = color.G * fade.Value / 255;
                    var b = color.B * fade.Value / 255;
                    currentColors[fade.Key] = Color.FromArgb(color.A, color.R - r, color.G - g, color.B - b);
                    keyFades[fade.Key]--;
                    if (keyFades[fade.Key] == 0)
                        toDelete.Add(fade.Key, fade.Value);
                }

                foreach (var key in toDelete)
                {
                    keyFades.Remove(key.Key);
                }
            }
        }

        public override void Info(int counter, long elapsedMilliseconds, IReadOnlyCollection<LedKey> pressed)
        {
            foreach (var press in pressed)
            {
                keyFades[press] = biggest;
            }
        }
    }
}
