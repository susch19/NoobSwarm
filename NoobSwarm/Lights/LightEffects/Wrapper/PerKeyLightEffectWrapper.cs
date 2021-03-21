using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects.Wrapper
{
    public class PerKeyLightEffectWrapper : LightEffectWrapper
    {
        public HashSet<LedKey> LedKeys { get; set; }

        public PerKeyLightEffectWrapper(HashSet<LedKey> keys) : base()
        {
            LedKeys = keys;
        }
        public PerKeyLightEffectWrapper(HashSet<LedKey> keys, LightEffect childEffect) : base(childEffect)
        {
            LedKeys = keys;
        }


        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (MainEffect is null || !Initialized)
                return;
            if (!MainEffect.Initialized)
                MainEffect.Init(LedKeyPoints!);

            if (MainEffect.InitNextFrame(counter, elapsedMilliseconds, stepInrease, pressed))
                foreach (var item in currentColors)
                {
                    if (!LedKeys.Contains(item.Key))
                        continue;
                    var col = MainEffect.NextFrame(item.Key, item.Value, counter, elapsedMilliseconds, stepInrease);
                    if (col.HasValue)
                        currentColors[item.Key] = col.Value;
                }
        }


        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            if (!LedKeys.Contains(key))
                return null;

            return MainEffect?.NextFrame(key, currentColor, counter, elapsedMilliseconds, stepInrease);
        }
    }
}
