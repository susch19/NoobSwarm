using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class SingleKeysColorEffect : LightEffect
    {
        public Dictionary<LedKey, Color> KeyColors { get; set; }

        public Color? NonSetKeyColor { get; set; }

        public SingleKeysColorEffect(Dictionary<LedKey, Color> keyColors)
        {
            KeyColors = keyColors;
            Initialized = true;
        }
        public SingleKeysColorEffect(Dictionary<LedKey, Color> keyColors, Color nonSetKeyColor)
        {
            KeyColors = keyColors;
            NonSetKeyColor = nonSetKeyColor;
            Initialized = true;

        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds)
        {
            if (NonSetKeyColor.HasValue)
            {
                foreach (var col in currentColors)
                {
                    if (!KeyColors.TryGetValue(col.Key, out var color))
                        color = NonSetKeyColor.Value;
                    currentColors[col.Key] = color;
                }
            }
            else
            {
                foreach (var keyColor in KeyColors)
                {
                    currentColors[keyColor.Key] = keyColor.Value;
                }
            }


        }
    }
}
