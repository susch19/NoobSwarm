
using Newtonsoft.Json;

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
        public KeyChangeState TriggerOn { get; set; } = KeyChangeState.Pressed;

        private Dictionary<LedKey, short> keyFades = new();

        public override bool InitNextFrame(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            foreach (var press in pressed)
            {
                if ((TriggerOn & press.state) > 0)
                    keyFades[press.key] = 0;
            }

            if (keyFades.Count == 0)
                return false;

            var step = (byte)Math.Min((stepInrease * Speed), 255);
            for (int i = keyFades.Count - 1; i >= 0; i--)
            {
                var keyFade = keyFades.ElementAt(i);
                if (keyFade.Value > 255)
                    keyFades.Remove(keyFade.Key);
                else
                    keyFades[keyFade.Key] += step;
            }


            return true;
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            if (!keyFades.TryGetValue(key, out var s))
                return null;

            return GetColorWithBrightness(Color.FromArgb(
                currentColor.A,
                (byte)(currentColor.R * s / 255),
                (byte)(currentColor.G * s / 255),
                (byte)(currentColor.B * s / 255)));
        }

        public override void Info(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            foreach (var press in pressed)
            {
                if (press.state == KeyChangeState.Pressed)
                    keyFades[press.key] = 0;
            }
        }

    }
}
