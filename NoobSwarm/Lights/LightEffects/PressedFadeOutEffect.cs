using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class PressedFadeOutEffect : LightEffect
    {
        public bool FasterPreKeyPress { get; set; }
        public KeyChangeState TriggerOn { get; set; }

        private Dictionary<LedKey, (short value, byte multiplier)> keyFades = new();

        public PressedFadeOutEffect(bool fasterPreKeyPress = false)
        {
            FasterPreKeyPress = fasterPreKeyPress;
            TriggerOn = KeyChangeState.Pressed;
        }

        public override bool InitNextFrame(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {

            UpdateKeyPressed(pressed);
            if (keyFades.Count == 0)
                return false;

            var step = (byte)Math.Min((stepInrease * Speed), 255);
            for (int i = keyFades.Count - 1; i >= 0; i--)
            {
                var keyFade = keyFades.ElementAt(i);

                var keyFadeVal = ((short)(keyFade.Value.value - step), keyFade.Value.multiplier);

                if (keyFadeVal.Item1 <= 0)
                    keyFades.Remove(keyFade.Key);
                else
                    keyFades[keyFade.Key] = keyFadeVal;
            }

            return true;
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            if (!keyFades.TryGetValue(key, out var s))
                return null;

            return GetColorWithBrightness(Color.FromArgb(
                currentColor.A,
                (byte)(currentColor.R * (s.value - s.multiplier) / 255),
                (byte)(currentColor.G * (s.value - s.multiplier) / 255),
                (byte)(currentColor.B * (s.value - s.multiplier) / 255)));

        }

        public override void Info(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            UpdateKeyPressed(pressed);
        }

        private void UpdateKeyPressed(IReadOnlyList<(LedKey key, KeyChangeState state)> keyStates)
        {
            foreach (var press in keyStates)
            {
                if ((TriggerOn & press.state) == 0)
                    continue;

                keyFades[press.key] = (255, 0);
                if (FasterPreKeyPress)
                {
                    for (int i = 0; i < keyFades.Count; i++)
                    {
                        var other = keyFades.ElementAt(i);
                        if (other.Key == press.key)
                            continue;

                        keyFades[other.Key] = (other.Value.value, (byte)Math.Min(255, other.Value.multiplier + 10));
                    }
                }
            }
        }
    }
}
