using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class SolidColorEffect : LightEffect
    {
        public Color? SolidColor { get; set; }
        public SolidColorEffect()
        {
        }
        public SolidColorEffect(Color? color)
        {
            SolidColor = color;
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            if (SolidColor.HasValue)
                foreach (var key in currentColors.Keys)
                {
                    currentColors[key] = SolidColor.Value;
                }
        }
    }
}
