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

        public IReadOnlyList<LightEffect> OverrideLightEffects => overrideLightEffects;

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
        private byte brightness;
        private bool updateWithBrightness;
        private readonly Task updateTask;
        private readonly VulcanKeyboard keyboard;
        private readonly List<LedKeyPoint> ledKeyPoints = new();
        private readonly List<LightEffect> lightLayers = new();
        private readonly List<LightEffect> overrideLightEffects = new();
        private readonly List<LedKey> pressedKeys = new();


        private readonly Dictionary<LedKey, Color> currentColors;

        public LightService(VulcanKeyboard keyboard)
        {
            this.keyboard = keyboard;
            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;
            currentColors = new Dictionary<LedKey, Color>() { };
            foreach (var ledKey in Enum.GetValues<LedKey>().Distinct())
            {
                if (!currentColors.ContainsKey((LedKey)(int)ledKey))
                    currentColors.Add(ledKey, Color.Black);

            }
            TargetUpdateRate = 30;

            ledKeyPoints = LedKeyPoint.LedKeyPoints.ToList();
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

        public void AddOverrideToEnd(LightEffect lightEffect)
        {
            overrideLightEffects.Add(lightEffect);
            lightEffect.Init(LedKeyPoints);

        }
        public void AddOverrideToStart(LightEffect lightEffect)
        {
            overrideLightEffects.Insert(0, lightEffect);
            lightEffect.Init(LedKeyPoints);
        }

        public void AddOverrideAtPosition(LightEffect lightEffect, int position)
        {
            overrideLightEffects.Insert(position, lightEffect);
            lightEffect.Init(LedKeyPoints);
        }

        public void RemoveLightEffect(LightEffect lightEffect)
        {
            lightLayers.Remove(lightEffect);
        }
        public void RemoveOverrideEffect(LightEffect lightEffect)
        {
            overrideLightEffects.Remove(lightEffect);
        }

        public void ClearOverrideEffects()
        {
            overrideLightEffects.Clear();
        }

        private void UpdateLoop()
        {
            Stopwatch sw = new Stopwatch();
            
            while (true)
            {
                sw.Restart();
                var pressed = pressedKeys.AsReadOnly();
                if (OverrideLightEffects.Count == 0)
                {
                    foreach (var lightEffect in LightLayers)
                    {
                        if (lightEffect.Initialized && lightEffect.Active)
                        {
                            lightEffect.Next(currentColors, Counter, ElapsedMilliseconds, Speed, pressed);
                            lightEffect.Info(Counter, ElapsedMilliseconds, Speed, pressed);
                        }
                    }
                }
                else
                {
                    foreach (var lightEffect in OverrideLightEffects)
                    {
                        if (lightEffect.Initialized && lightEffect.Active)
                            lightEffect.Next(currentColors, Counter, ElapsedMilliseconds, Speed, pressed);
                    }
                    foreach (var lightEffect in LightLayers)
                    {
                        if (lightEffect.Initialized && lightEffect.Active)
                        {
                            lightEffect.Info(Counter, ElapsedMilliseconds, Speed, pressed);
                        }
                    }
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
        private void Keyboard_KeyPressedReceived(object? sender, KeyPressedArgs e)
        {
            if (e.IsPressed)
                pressedKeys.Add(e.Key);
            else
                pressedKeys.Remove(e.Key);
        }
    }
}
