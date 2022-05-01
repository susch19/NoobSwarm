

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Lights.LightEffects.Wrapper;
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
        [JsonIgnore]
        public IReadOnlyList<LightEffectWrapper> LightLayers => lightLayers;

        [JsonIgnore]
        public long ElapsedMilliseconds { get => elapsedMilliseconds; private set => elapsedMilliseconds = value; }
        [JsonIgnore]
        public int Counter { get => counter; private set => counter = value; }
        public bool Reversed { get; set; }
        public short Speed { get; set; }

        [JsonIgnore]
        public IReadOnlyList<LightEffectWrapper> OverrideLightEffects => overrideLightEffects;

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

        [JsonIgnore]
        public IReadOnlyList<LedKeyPoint> LedKeyPoints => ledKeyPoints;

        private int msSleep;
        private int targetUpdateRate;
        private byte brightness;
        private bool updateWithBrightness = true;
        private IVulcanKeyboard? keyboard;
        [JsonProperty]
        private long elapsedMilliseconds;
        [JsonProperty]
        private int counter;
        private readonly List<LedKeyPoint> ledKeyPoints = new();
        [JsonProperty]
        private readonly List<LightEffectWrapper> lightLayers = new();
        [JsonProperty]
        private readonly List<LightEffectWrapper> overrideLightEffects = new();
        private readonly List<LedKey> pressedKeys = new();
        private readonly List<LedKey> pressedKeysToRemove = new();
        [JsonProperty]
        private readonly Dictionary<LedKey, Color> currentColors = new();

        public LightService()
        {
            keyboard = TypeContainer.Get<IVulcanKeyboard>();
            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;
            if (keyboard is ILedKeyPointProvider provider)
                ledKeyPoints.AddRange(provider.GetLedKeyPoints());
            else
                ledKeyPoints = LedKeyPoint.LedKeyPoints.ToList();
            foreach (var ledKey in ledKeyPoints.Select(x => x.LedKey).Distinct())
            {
                if (!currentColors.ContainsKey((LedKey)(int)ledKey))
                    currentColors.Add(ledKey, Color.Black);

            }
            Brightness = 255;
            Speed = 5;
            TargetUpdateRate = 60;
        }

        public LightService(IVulcanKeyboard keyboard)
        {
            this.keyboard = keyboard;
            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;
            foreach (var ledKey in ledKeyPoints.Select(x => x.LedKey).Distinct())
            {
                if (!currentColors.ContainsKey((LedKey)(int)ledKey))
                    currentColors.Add(ledKey, Color.Black);

            }

            if (keyboard is ILedKeyPointProvider provider)
                ledKeyPoints.AddRange(provider.GetLedKeyPoints());
            else
                ledKeyPoints = LedKeyPoint.LedKeyPoints.ToList();
            Brightness = 255;
            Speed = 5;
            TargetUpdateRate = 60;
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
            return new LightService();
            if (!File.Exists("LightConfig.save"))
                return TypeContainer.CreateObject<LightService>();

            using var fs = File.OpenRead("LightConfig.save");
            using var reader = new BsonDataReader(fs);
            var service = SerializationHelper.TypeSafeSerializer.Deserialize<LightService>(reader);
            if (service is null)
                service = TypeContainer.CreateObject<LightService>();

            return service;
        }


        public void AddToEnd(LightEffectWrapper lightEffect)
        {
            lightLayers.Add(lightEffect);
            lightEffect.Init(LedKeyPoints);

        }
        public void AddToStart(LightEffectWrapper lightEffect)
        {
            if (lightLayers.Count > 0)
                lightLayers.Insert(0, lightEffect);
            else
                lightLayers.Add(lightEffect);
            lightEffect.Init(LedKeyPoints);
        }

        public void AddAtPosition(LightEffectWrapper lightEffect, int position)
        {
            lightLayers.Insert(position, lightEffect);
            lightEffect.Init(LedKeyPoints);
        }

        public void AddOverrideToEnd(LightEffectWrapper lightEffect)
        {
            overrideLightEffects.Add(lightEffect);
            lightEffect.Init(LedKeyPoints);

        }
        public void AddOverrideToStart(LightEffectWrapper lightEffect)
        {
            overrideLightEffects.Insert(0, lightEffect);
            lightEffect.Init(LedKeyPoints);
        }

        public void AddOverrideAtPosition(LightEffectWrapper lightEffect, int position)
        {
            overrideLightEffects.Insert(position, lightEffect);
            lightEffect.Init(LedKeyPoints);
        }

        public void RemoveLightEffect(LightEffectWrapper lightEffect)
        {
            lightLayers.Remove(lightEffect);
        }
        public void RemoveOverrideEffect(LightEffectWrapper lightEffect)
        {
            overrideLightEffects.Remove(lightEffect);
        }

        public bool Contains(LightEffectWrapper effect) => lightLayers.Contains(effect);


        public void ClearOverrideEffects()
        {
            overrideLightEffects.Clear();
        }

        public void UpdateLoop(CancellationToken token)
        {
            Stopwatch sw = new Stopwatch();
            Thread.CurrentThread.Name = "LightService_UpdateLoop";
            List<(LedKey key, KeyChangeState state)> pressedCopy = new();
            Dictionary<LedKey, Color> currentColorsCopy = new();
            List<LightEffectWrapper> overrideLayersCopy = new();
            List<LightEffectWrapper> lightLayersCopy = new();

            foreach (var eff in LightLayers.Union(OverrideLightEffects))
            {
                if (!eff.Initialized)
                    eff.Init(ledKeyPoints);
            }
            while (!token.IsCancellationRequested)
            {
                if (keyboard is null || currentColors is null)
                {
                    Thread.Sleep(16);
                    continue;
                }

                sw.Restart();

                for (int i = pressedCopy.Count - 1; i >= 0; i--)
                {
                    var item = pressedCopy[i];
                    if (item.state == KeyChangeState.Release)
                    {
                        pressedCopy.RemoveAt(i);
                        continue;
                    }

                    if (pressedKeysToRemove.Contains(item.key))
                        pressedCopy[i] = (item.key, KeyChangeState.Release);
                    else if (pressedKeys.Contains(item.key))
                        pressedCopy[i] = (item.key, KeyChangeState.Hold);
                }

                foreach (var toRemove in pressedKeysToRemove.ToList())
                {
                    pressedKeys.Remove(toRemove);
                }
                pressedKeysToRemove.Clear();
                foreach (var item in pressedKeys.ToList())
                {
                    if (pressedCopy.Any(x => x.key == item))
                        continue;
                    pressedCopy.Add((item, KeyChangeState.Pressed));
                }

                foreach (var copy in currentColors)
                    currentColorsCopy[copy.Key] = copy.Value;
                overrideLayersCopy.AddRange(OverrideLightEffects);
                lightLayersCopy.AddRange(LightLayers);

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
                            catch (Exception ex)
                            {
                            }
                            //try
                            //{
                            //    lightEffect.Info(Counter, ElapsedMilliseconds, Speed, pressedCopy);
                            //}
                            //catch (Exception ex)
                            //{
                            //}

                        }
                    }
                }
                else
                {
                    foreach (var lightEffect in overrideLayersCopy)
                    {
                        if (lightEffect is not null && lightEffect.Initialized && lightEffect.Active)
                        {
                            try
                            {
                                lightEffect.Next(currentColorsCopy, Counter, ElapsedMilliseconds, Speed, pressedCopy);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    //foreach (var lightEffect in lightLayersCopy)
                    //{
                    //    if (lightEffect.Initialized && lightEffect.Active)
                    //    {
                    //        try
                    //        {
                    //            lightEffect.Info(Counter, ElapsedMilliseconds, Speed, pressedCopy);
                    //        }
                    //        catch
                    //        {
                    //        }
                    //    }
                    //}
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

                currentColorsCopy.Clear();

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
