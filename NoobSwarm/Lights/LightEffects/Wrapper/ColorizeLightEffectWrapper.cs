using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects.Wrapper
{
    public class ColorizeLightEffectWrapper : LightEffectWrapper
    {
        public LightEffect? ColorEffect { get; set; }

        public ColorizeLightEffectWrapper()
        {

        }
        public ColorizeLightEffectWrapper(LightEffect childEffect, LightEffect colorEffect) : base(childEffect)
        {
            ColorEffect = colorEffect;
        }


        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (MainEffect is null || ColorEffect is null || !Initialized || LedKeyPoints is null)
                return;
            if (!MainEffect.Initialized)
                MainEffect.Init(LedKeyPoints);
            if (!ColorEffect.Initialized)
                ColorEffect.Init(LedKeyPoints);

            if (ColorEffect.InitNextFrame(counter, elapsedMilliseconds, stepInrease, pressed) &&
               MainEffect.InitNextFrame(counter, elapsedMilliseconds, stepInrease, pressed))
                foreach (var item in currentColors)
                {
                    var col = ColorEffect.NextFrame(item.Key, item.Value, counter, elapsedMilliseconds, stepInrease);
                    if (col.HasValue)
                        col = MainEffect.NextFrame(item.Key, col.Value, counter, elapsedMilliseconds, stepInrease);
                    if (col.HasValue)
                        currentColors[item.Key] = col.Value;
                }
        }

        public override bool InitNextFrame(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (MainEffect is null || ColorEffect is null || !Initialized || LedKeyPoints is null)
                return false;
            if (!MainEffect.Initialized)
                MainEffect.Init(LedKeyPoints);
            if (!ColorEffect.Initialized)
                ColorEffect.Init(LedKeyPoints);

            if (ColorEffect.InitNextFrame(counter, elapsedMilliseconds, stepInrease, pressed))
                return MainEffect.InitNextFrame(counter, elapsedMilliseconds, stepInrease, pressed);
            return false;
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            var col = ColorEffect?.NextFrame(key, currentColor, counter, elapsedMilliseconds, stepInrease);
            if (!col.HasValue)
                return col;
            return MainEffect?.NextFrame(key, col.Value, counter, elapsedMilliseconds, stepInrease);
        }
    }
}
