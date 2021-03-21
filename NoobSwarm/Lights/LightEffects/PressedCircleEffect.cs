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

        public bool ShouldClearKeys { get; set; }

        private Dictionary<LedKey, float> keyDistances = new();
        private short step;

        public KeyChangeState TriggerOnState { get; set; }
        public bool Reverse { get; set; } = false;

        public PressedCircleEffect()
        {
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

        public override bool InitNextFrame(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (LedKeyPoints is null)
                return false;
            keyDistances.Clear();

            for (int i = wavePositions.Count - 1; i >= 0; i--)
            {
                var (waveKeyStart, waveRadius) = wavePositions[i];
                if ((step < 0 && waveRadius + step < 0)
                    || waveRadius + step > maxRadius)
                {
                    wavePositions.RemoveAt(i);
                    continue;
                }
                wavePositions[i] = (waveKeyStart, waveRadius + stepInrease);

            }

            step = stepInrease;
            if (Reverse)
                step *= -1;

            foreach (var press in pressed)
            {
                if ((press.Item2 & TriggerOnState) > 0)
                {
                    var keyPoint = LedKeyPoints.FirstOrDefault(x => x.LedKey == press.Item1);
                    if (step < 0)
                    {
                        int startRadius = maxRadius - keyPoint.X;
                        wavePositions.Add((keyPoint, startRadius > keyPoint.X ? startRadius : keyPoint.X));
                    }
                    else
                    {
                        wavePositions.Add((keyPoint, 1));

                    }

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
                        keyDistances[key.LedKey] = t;
                    }
                }
            }
            return true;
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            if (!keyDistances.TryGetValue(key, out var t))
                return null;

            return GetColorWithBrightness(Color.FromArgb(currentColor.A,
                 Math.Clamp((int)(currentColor.R  * (1 - t)), 0, 255),
                    Math.Clamp((int)(currentColor.G  * (1 - t)), 0, 255),
                    Math.Clamp((int)(currentColor.B  * (1 - t)), 0, 255)
                ));
        }
    }
}
