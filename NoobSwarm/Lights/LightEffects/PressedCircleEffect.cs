using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class PressedCircleEffect : LightEffect
    {



        private List<int> xPoses;
        private List<int> yPoses;
        private int maxRadius;
        private List<(LedKeyPoint waveKeyStart, int waveRadius)> wavePositions = new();
        //private Dictionary<(int x, int y), LedKey> pointToKeys = new();

        public Color? WaveColor { get; set; }
        public bool ShouldClearKeys { get; set; }

        private LightEffect? effect;
        private Dictionary<LedKey, Color> keyColors = new();
        private Dictionary<LedKey, float> keyDistances = new();

        public KeyChangeState TriggerOnState { get; set; }

        public PressedCircleEffect()
        {
            WaveColor = Color.White;
            TriggerOnState = KeyChangeState.Pressed;
        }

        public PressedCircleEffect(Color color)
        {
            WaveColor = color;
            TriggerOnState = KeyChangeState.Pressed;

        }
        public PressedCircleEffect(LightEffect effect)
        {
            this.effect = effect;
            TriggerOnState = KeyChangeState.Pressed;

        }


        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            base.Init(ledKeyPoints);
            xPoses = ledKeyPoints.Select(x => x.X).Distinct().OrderBy(x => x).ToList();
            yPoses = ledKeyPoints.Select(x => x.Y).Distinct().OrderBy(x => x).ToList();
            //pointToKeys = ledKeyPoints.ToDictionary(x => (x.X, x.Y), x => x.LedKey);
            maxRadius = xPoses.Union(yPoses).Max();
            maxRadius += maxRadius * 5 / 100;

        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (LedKeyPoints is null)
                return;
            keyColors.Clear();
            if (effect is not null)
            {
                if (!effect.Initialized)
                    effect?.Init(LedKeyPoints);
            }

            foreach (var press in pressed)
            {
                if ((press.Item2 & TriggerOnState) > 0)
                {
                    var keyPoint = LedKeyPoints.FirstOrDefault(x => x.LedKey == press.Item1);
                    wavePositions.Add((keyPoint, 1));
                }
            }



            foreach (var key in LedKeyPoints)
            {

                for (int i = wavePositions.Count - 1; i >= 0; i--)
                {
                    var (waveKeyStart, waveRadius) = wavePositions[i];

                    var x = key.X - waveKeyStart.X;
                    var y = key.Y - waveKeyStart.Y;

                    var l = MathF.Sqrt(x * x + y * y);
                    var t = Math.Clamp(Math.Abs(waveRadius - l) / 20f, 0, 1f);

                    if (t < 1)
                    {
                        keyColors[key.LedKey] = currentColors[key.LedKey];
                        keyDistances[key.LedKey] = t;
                    }
                }
            }

            if (effect is not null)
                effect.Next(keyColors, counter, elapsedMilliseconds, stepInrease, pressed);

            foreach (var keyColor in keyColors)
            {
                var prevColor = keyColor.Value;
                var t = keyDistances[keyColor.Key];

                if (effect is null && WaveColor is not null)
                {
                    Color c = Color.FromArgb(prevColor.A,
                        Math.Clamp((int)(prevColor.R * t + WaveColor.Value.R * (1 - t)), 0, 255),
                        Math.Clamp((int)(prevColor.G * t + WaveColor.Value.G * (1 - t)), 0, 255),
                        Math.Clamp((int)(prevColor.B * t + WaveColor.Value.B * (1 - t)), 0, 255));
                    currentColors[keyColor.Key] = c;
                }
                else if (effect is not null)
                {
                    currentColors[keyColor.Key] = keyColor.Value;
                }
            }

            for (int i = wavePositions.Count - 1; i >= 0; i--)
            {
                var (waveKeyStart, waveRadius) = wavePositions[i];
                if (waveRadius + stepInrease > maxRadius)
                {
                    wavePositions.RemoveAt(i);
                    continue;
                }
                wavePositions[i] = (waveKeyStart, waveRadius + stepInrease);
            }
        }
    }
}
