﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class RandomColorPerKeyEffect : LightEffect
    {
        private Random random = new();

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            foreach (var key in currentColors.Keys)
            {
                currentColors[key] = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
            }
        }
    }
}
