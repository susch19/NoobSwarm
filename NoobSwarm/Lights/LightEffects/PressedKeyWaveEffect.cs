using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class PressedKeyWaveEffect : LightEffect
    {


        record KeyState(int Iteration, LedKey Key, Direction Direction);

        public KeyChangeState TriggerOn { get; set; } = KeyChangeState.Pressed;

        private List<int> xPoses=new();
        private List<int> yPoses=new();
        private List<(LedKeyPoint start, int state)> wavePositions = new();
        private Dictionary<LedKey, float> keyDistances = new();

        public bool ShouldClearKeys { get; set; }

        private KeyState[]? keystates;

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            base.Init(ledKeyPoints);
            xPoses = ledKeyPoints.Select(x => x.X).Distinct().OrderBy(x => x).ToList();
            yPoses = ledKeyPoints.Select(x => x.Y).Distinct().OrderBy(x => x).ToList();
            //pointToKeys = ledKeyPoints.ToDictionary(x => (x.X, x.Y), x => x.LedKey);
            keystates = new KeyState[xPoses.Count * yPoses.Count];
            for (int x = 0; x < xPoses.Count; x++)
                for (int y = 0; y < yPoses.Count; y++)
                {
                    var keyPoint = ledKeyPoints.FirstOrDefault(a => a.X == xPoses[x] && a.Y == yPoses[y]);
                    LedKey ledKey = keyPoint.LedKey;
                    if (keyPoint == default(LedKeyPoint) && (x > 0 || y > 0))
                        ledKey = (LedKey)(-1);

                    keystates[GetIndex(x, y)] = new(0, ledKey, Direction.None);
                    if (ledKey != (LedKey)(-1))
                        ;
                }

        }

        public override bool InitNextFrame(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (keystates is null || LedKeyPoints is null)
                return false;
            keyDistances.Clear();

            foreach (var press in pressed)
            {
                if (press.state != KeyChangeState.Pressed)
                    continue;

                var keyPoint = LedKeyPoints.FirstOrDefault(x => x.LedKey == press.key);
                var x = xPoses.IndexOf(keyPoint.X);
                var y = yPoses.IndexOf(keyPoint.Y);
                keystates[GetIndex(x, y)] = keystates[GetIndex(x, y)] with { Direction = Direction.Left | Direction.Up | Direction.Down | Direction.Right, Iteration = counter };
            }
            for (int x = 0; x < xPoses.Count; x++)
                for (int y = 0; y < yPoses.Count; y++)
                {
                    int index = GetIndex(x, y);
                    var state = keystates[index];
                    if (state.Direction == Direction.None || state.Iteration == counter)
                        continue;

                    if ((state.Direction & Direction.Up) > 0)
                    {
                        if (y > 0)
                        {
                            var subIndex = GetIndex(x, y - 1);
                            var kstate = keystates[subIndex];

                            keystates[subIndex] = kstate with { Direction = (Direction.Up | Direction.Right | Direction.Left) & state.Direction, Iteration = counter };
                            state = state with { Direction = state.Direction & ~Direction.Up };
                        }
                    }

                    if ((state.Direction & Direction.Left) > 0)
                    {
                        if (x > 0)
                        {
                            var subIndex = GetIndex(x - 1, y);
                            var kstate = keystates[subIndex];

                            keystates[subIndex] = kstate with { Direction = (Direction.Up | Direction.Down | Direction.Left) & state.Direction, Iteration = counter };
                            state = state with { Direction = state.Direction & ~Direction.Left };
                        }
                    }


                    if ((state.Direction & Direction.Right) > 0)
                    {
                        if (x < xPoses.Count - 1)
                        {
                            var subIndex = GetIndex(x + 1, y);
                            var kstate = keystates[subIndex];

                            keystates[subIndex] = kstate with { Direction = (Direction.Up | Direction.Right | Direction.Down) & state.Direction, Iteration = counter };
                            state = state with { Direction = state.Direction & ~Direction.Right };
                        }
                    }
                    if ((state.Direction & Direction.Down) > 0)
                    {
                        if (y < yPoses.Count - 1)
                        {
                            var subIndex = GetIndex(x, y + 1);
                            var kstate = keystates[subIndex];

                            keystates[subIndex] = kstate with { Direction = (Direction.Down | Direction.Right | Direction.Left) & state.Direction, Iteration = counter };
                            state = state with { Direction = state.Direction & ~Direction.Down };
                        }
                    }
                    keystates[index] = state;
                }

            for (int x = 0; x < xPoses.Count; x++)
                for (int y = 0; y < yPoses.Count; y++)
                {
                    int index = GetIndex(x, y);
                    var realX = xPoses[x];
                    var realY = yPoses[y];

                    var state = keystates[index];

                    if (state.Iteration != counter)
                        continue;

                    KeyState? currentClosest = null;
                    int currentDiffSq = int.MaxValue;
                    if (state.Key != (LedKey)(-1))
                    {
                        currentClosest = state;
                        currentDiffSq = 0;
                    }
                    else
                        for (int xDiff = -2; xDiff <= 2; xDiff++)
                        {
                            for (int yDiff = -2; yDiff <= 2; yDiff++)
                            {
                                int tX = x + xDiff;
                                int tY = y + yDiff;
                                if (tX < 0 || tX >= xPoses.Count || tY < 0 || tY >= yPoses.Count)
                                    continue;

                                var tmp = keystates[GetIndex(tX, tY)];
                                if (tmp.Key == (LedKey)(-1))
                                    continue;
                                var realTX = xPoses[tX];
                                var realTY = yPoses[tY];
                                int tXDiff = (realX - realTX);
                                int tYDiff = (realY - realTY);
                                int tDiffSq = tXDiff * tXDiff + tYDiff * tYDiff;
                                if (tDiffSq < currentDiffSq)
                                {
                                    currentDiffSq = tDiffSq;
                                    currentClosest = tmp;
                                }
                            }
                        }
                    if (currentClosest != null)
                    {
                        if (state.Iteration == counter)
                        {
                            float t = Math.Clamp(MathF.Sqrt(currentDiffSq) / 25, 0, 1);

                            keyDistances[currentClosest.Key] = t;

                        }
                    }
                }


            return true;
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {

            var keyPoint = LedKeyPoints!.FirstOrNull(x => x.LedKey == key);

            if (keyPoint is null)
                return null;

            if (!keyDistances.TryGetValue(key, out var t))
                return null;

            return GetColorWithBrightness(Color.FromArgb(currentColor.A,
                Math.Clamp((int)(currentColor.R * (1 - t)), 0, 255),
                Math.Clamp((int)(currentColor.G * (1 - t)), 0, 255),
                Math.Clamp((int)(currentColor.B * (1 - t)), 0, 255)));
        }

        private int GetIndex(int x, int y) => (x * yPoses.Count) + y;
    }
}
