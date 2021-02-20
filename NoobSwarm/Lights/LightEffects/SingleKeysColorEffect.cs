﻿using System;
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

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            if (NonSetKeyColor.HasValue)
            {
                foreach (var col in currentColors)
                {
                    if (!KeyColors.TryGetValue(col.Key, out var color))
                        color = NonSetKeyColor.Value;
                    currentColors[col.Key] = Color.FromArgb(color.A, (byte)(color.R * brightnessPercent), (byte)(color.G * brightnessPercent), (byte)(color.B * brightnessPercent)); 
                }
            }
            else
            {
                foreach (var keyColor in KeyColors)
                {
                    if (currentColors.ContainsKey(keyColor.Key))
                        currentColors[keyColor.Key] = Color.FromArgb(keyColor.Value.A, (byte)(keyColor.Value.R * brightnessPercent), (byte)(keyColor.Value.G * brightnessPercent), (byte)(keyColor.Value.B * brightnessPercent));
                }
            }


        }
    }
}
