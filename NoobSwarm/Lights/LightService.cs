using NoobSwarm.Lights.LightEffects;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights
{
    public class LightService
    {
        public IReadOnlyList<LightEffect> LightLayers => lightLayers;

        public long ElapsedMilliseconds { get; private set; }
        public int Counter { get; private set; }
        public bool Reversed { get; set; }
        public ushort Speed { get; set; }

        public LightEffect? OverrideLightEffect
        {
            get => overrideLightEffect; set
            {
                if (value == overrideLightEffect)
                    return;
                overrideLightEffect = value;
                //msSleep = 16; //Increase Target rate, so the light feels more responsive

                //if (value is null)
                //    TargetUpdateRate = TargetUpdateRate; //Reset Target rate and return to default mode
            }
        }
        public byte Brightness
        {
            get => brightness; set
            {
                if (value == brightness)
                    return;
                brightness = value;
                updateWithBrightness = true;
            }
        }


        public int TargetUpdateRate
        {
            get => targetUpdateRate; set
            {
                if (value == targetUpdateRate)
                    return;

                targetUpdateRate = value;
                msSleep = 1000 / TargetUpdateRate;
            }
        }

        public IReadOnlyList<LedKeyPoint> LedKeyPoints => ledKeyPoints;

        private int msSleep;
        private int targetUpdateRate;
        private LightEffect overrideLightEffect;
        private byte brightness;
        private bool updateWithBrightness;
        private readonly Task updateTask;
        private readonly VulcanKeyboard keyboard;
        private readonly List<LedKeyPoint> ledKeyPoints = new();
        private readonly List<LightEffect> lightLayers = new();


        private readonly Dictionary<LedKey, Color> currentColors;

        public LightService(VulcanKeyboard keyboard)
        {
            this.keyboard = keyboard;
            currentColors = new Dictionary<LedKey, Color>() { };
            foreach (var ledKey in Enum.GetValues<LedKey>().Distinct())
            {
                if (!currentColors.ContainsKey((LedKey)(int)ledKey))
                    currentColors.Add(ledKey, Color.Black);

            }
            TargetUpdateRate = 30;

            var lines = File.ReadAllLines("Assets/LedPositions.txt").Select(x => x.AsMemory());

            foreach (var line in lines)
            {
                var span = line.Span;
                var firstIndex = span.IndexOf('|');
                var lastIndex = span.LastIndexOf('|');
                var x = int.Parse(span.Slice(0, firstIndex));
                var y = int.Parse(span.Slice(firstIndex + 1, lastIndex - firstIndex - 1));
                var ledKey = Enum.Parse<LedKey>(span.Slice(lastIndex + 1, span.Length - lastIndex - 1).ToString());
                ledKeyPoints.Add(new LedKeyPoint(x, y, ledKey));
            }
            Speed = 1;
            updateTask = Task.Run(UpdateLoop);
        }


        public void AddToEnd(LightEffect lightEffect)
        {
            lightLayers.Add(lightEffect);
            lightEffect.Init(LedKeyPoints);

        }
        public void AddToStart(LightEffect lightEffect)
        {
            lightLayers.Insert(0, lightEffect);
            lightEffect.Init(LedKeyPoints);
        }

        public void AddAtPosition(LightEffect lightEffect, int position)
        {
            lightLayers.Insert(position, lightEffect);
            lightEffect.Init(LedKeyPoints);
        }

        public void RemoveLightEffect(LightEffect lightEffect)
        {
            lightLayers.Remove(lightEffect);
        }

        private void UpdateLoop()
        {
            Stopwatch sw = new Stopwatch();

            while (true)
            {
                sw.Restart();
                var copy = OverrideLightEffect;
                if (copy is null)
                {
                    foreach (var lightEffect in LightLayers)
                    {
                        if (lightEffect.Initialized)
                            lightEffect.Next(currentColors, Counter, ElapsedMilliseconds);
                    }
                }
                else
                {
                    if (copy.Initialized)
                        copy.Next(currentColors, Counter, ElapsedMilliseconds);
                }
                keyboard.SetColors(currentColors);
                if (updateWithBrightness)
                {
                    updateWithBrightness = false;
                    keyboard.Brightness = Brightness;
                }
                else
                {
                    keyboard.Update();
                }

                sw.Stop();
                if (sw.ElapsedMilliseconds > msSleep)
                    ElapsedMilliseconds += sw.ElapsedMilliseconds;
                else
                    ElapsedMilliseconds += msSleep - sw.ElapsedMilliseconds;
                if (Reversed)
                    Counter -= Speed;
                else
                    Counter += Speed;

                if (sw.ElapsedMilliseconds < msSleep)
                    Thread.Sleep(msSleep - (int)sw.ElapsedMilliseconds);
            }
        }
    }
}
