﻿

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Serializations;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights
{
    public class LightService
    {
        public IReadOnlyList<LightEffect> LightLayers => lightLayers;

        [JsonIgnore]
        public long ElapsedMilliseconds { get => elapsedMilliseconds; private set => elapsedMilliseconds = value; }
        [JsonIgnore]
        public int Counter { get => counter; private set => counter = value; }
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
        private bool updateWithBrightness = true;
        private VulcanKeyboard? keyboard;
        [JsonProperty]
        private long elapsedMilliseconds;
        [JsonProperty]
        private int counter;
        private readonly List<LedKeyPoint> ledKeyPoints = new();
        [JsonProperty]
        private readonly List<LightEffect> lightLayers = new();
        [JsonProperty]
        private readonly List<LightEffect> overrideLightEffects = new();
        private readonly List<LedKey> pressedKeys = new();
        private readonly List<LedKey> pressedKeysToRemove = new();
        [JsonProperty]
        private readonly Dictionary<LedKey, Color> currentColors = new();

        public LightService()
        {
            keyboard = TypeContainer.Get<VulcanKeyboard>();
            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;
            ledKeyPoints.AddRange(LedKeyPoint.LedKeyPoints);
        }

        public LightService(VulcanKeyboard keyboard)
        {
            this.keyboard = keyboard;
            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;
            foreach (var ledKey in Enum.GetValues<LedKey>().Distinct())
            {
                if (!currentColors.ContainsKey((LedKey)(int)ledKey))
                    currentColors.Add(ledKey, Color.Black);

            }
            TargetUpdateRate = 30;

            ledKeyPoints = LedKeyPoint.LedKeyPoints.ToList();
            Speed = 1;
        }

        //public void Serialize()
        //{
        //    //using var fs = File.OpenWrite("LightConfig.save");
        //    //MessagePackSerializer.Serialize(fs, this);
        //}
        //public static LightService Deserialize()
        //{
        //    if (!File.Exists("LightConfig.save"))
        //        return TypeContainer.CreateObject<LightService>();
        //    using var fs = File.OpenRead("LightConfig.save");
        //    return MessagePackSerializer.Deserialize<LightService>(fs);
        //}

        public void Serialize()
        {
            using var fs = File.OpenWrite("LightConfig.save");
            using var writer = new BsonDataWriter(fs);
            SerializationHelper.TypeSafeSerializer.Serialize(writer, this);
        }
        public static LightService Deserialize()
        {
            if (!File.Exists("LightConfig.save"))
                return TypeContainer.CreateObject<LightService>();

            using var fs = File.OpenRead("LightConfig.save");
            using var reader = new BsonDataReader(fs);
            var service = SerializationHelper.TypeSafeSerializer.Deserialize<LightService>(reader);
            if (service is null)
                service = TypeContainer.CreateObject<LightService>();
            return service;
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

        public bool Contains(LightEffect effect) => lightLayers.Contains(effect);


        public void ClearOverrideEffects()
        {
            overrideLightEffects.Clear();
        }

        public void UpdateLoop(CancellationToken token)
        {
            Stopwatch sw = new Stopwatch();
            Thread.CurrentThread.Name = "LightService_UpdateLoop";
            List<LedKey> pressedCopy = new();
            Dictionary<LedKey, Color> currentColorsCopy = new();
            List<LightEffect> overrideLayersCopy = new();
            List<LightEffect> lightLayersCopy = new();
            while (!token.IsCancellationRequested)
            {
                if (keyboard is null || currentColors is null)
                {
                    Thread.Sleep(16);
                    continue;
                }

                sw.Restart();
                pressedCopy.AddRange(pressedKeys);
                foreach (var copy in currentColors)
                    currentColorsCopy[copy.Key] = copy.Value;
                overrideLayersCopy.AddRange(OverrideLightEffects);
                lightLayersCopy.AddRange(LightLayers);

                var pressed = pressedKeys.AsReadOnly();
                if (overrideLayersCopy.Count == 0)
                {
                    foreach (var lightEffect in lightLayersCopy)
                    {
                        if (lightEffect.Initialized && lightEffect.Active)
                        {
                            try
                            {
                                lightEffect.Next(currentColorsCopy, Counter, ElapsedMilliseconds, Speed, pressedCopy);
                            }
                            catch
                            {
                            }
                            try
                            {
                                lightEffect.Info(Counter, ElapsedMilliseconds, Speed, pressedCopy);
                            }
                            catch
                            {
                            }

                        }
                    }
                }
                else
                {
                    foreach (var lightEffect in overrideLayersCopy)
                    {
                        if (lightEffect.Initialized && lightEffect.Active)
                        {
                            try
                            {
                                lightEffect.Next(currentColorsCopy, Counter, ElapsedMilliseconds, Speed, pressedCopy);
                            }
                            catch
                            {
                            }
                        }
                    }
                    foreach (var lightEffect in lightLayersCopy)
                    {
                        if (lightEffect.Initialized && lightEffect.Active)
                        {
                            try
                            {
                                lightEffect.Info(Counter, ElapsedMilliseconds, Speed, pressedCopy);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                keyboard.SetColors(currentColorsCopy);
                if (updateWithBrightness)
                {
                    updateWithBrightness = false;
                    keyboard.Brightness = Brightness;
                }
                else
                {
                    keyboard.Update();
                }

                foreach (var copy in currentColorsCopy)
                    currentColors[copy.Key] = copy.Value;

                pressedCopy.Clear();
                currentColorsCopy.Clear();
                foreach (var toRemove in pressedKeysToRemove)
                {
                    pressedKeys.Remove(toRemove);
                }
                pressedKeysToRemove.Clear();
                overrideLayersCopy.Clear();
                lightLayersCopy.Clear();

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
                pressedKeysToRemove.Add(e.Key);
        }
    }
}
