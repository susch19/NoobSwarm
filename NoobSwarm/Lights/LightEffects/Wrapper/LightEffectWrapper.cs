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
    public class LightEffectWrapper : LightEffect
    {
        public LightEffect? MainEffect { get; set; }

        public LightEffectWrapper()
        {

        }
        public LightEffectWrapper(LightEffect childEffect)
        {
            MainEffect = childEffect;
        }

        public virtual void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            LedKeyPoints = ledKeyPoints;
            Initialized = true;
        }

        public virtual void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (MainEffect is null || !Initialized)
                return;
            if (!MainEffect.Initialized)
                MainEffect.Init(LedKeyPoints!);

            if (MainEffect.InitNextFrame(counter, elapsedMilliseconds, stepInrease, pressed))
                foreach (var item in currentColors)
                {
                    var col = MainEffect.NextFrame(item.Key, item.Value, counter, elapsedMilliseconds, stepInrease);
                    if (col.HasValue)
                        currentColors[item.Key] = col.Value;
                }
        }

        public override bool InitNextFrame(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (MainEffect is null || !Initialized)
                return false;
            if (!MainEffect.Initialized)
                MainEffect.Init(LedKeyPoints!);

            return MainEffect.InitNextFrame(counter, elapsedMilliseconds, stepInrease, pressed);
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            return MainEffect?.NextFrame(key, currentColor, counter, elapsedMilliseconds, stepInrease);
        }
    }
}
