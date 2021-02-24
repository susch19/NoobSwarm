﻿using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm
{
    class Program
    {
        private static Task lightLoop;

        private static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }

        static void Main(string[] args)
        {
            using var keyboard = VulcanKeyboard.Initialize();
            var ls = new LightService(keyboard);
            var manager = new HotKeyManager(keyboard, ls, LedKey.FN_Key);
            ls.AddToEnd(new HSVColorWanderEffect());
            //ls.AddToEnd(new HSVColorWanderEffect(Enum.GetValues<LedKey>().Where(x => x.ToString().Length == 1).ToList(), new List<Color> { Color.Orange, Color.Green, Color.Red }) { Direction = Direction.Up, Speed = 1 });
            //ls.AddToEnd(new HSVColorWanderEffect(Enum.GetValues<LedKey>().Where(x => x.ToString()[0] == 'F' && x.ToString().Length is <= 3 and > 1).ToList()) { Direction = Direction.Right });
            //ls.AddToEnd(new HSVColorWanderEffect(Enum.GetValues<LedKey>().Where(x => x.ToString()[0] == 'D' && x.ToString().Length == 2).ToList()) { Direction = Direction.Left });
            //ls.AddToEnd(new BreathingColorPerKeyEffect(Enum.GetValues<LedKey>().Where(x => (byte)x >= 113).ToList(), new HSVColorWanderEffect()) { Speed = .1f });
            //ls.AddToEnd(new SingleKeysColorEffect(new() { { LedKey.ESC, Color.White } }));
            ls.AddToEnd(new SolidColorEffect() { Brightness = 50 });
            ls.AddToEnd(new PressedFadeOutEffect(new HSVColorWanderEffect() {Direction = Direction.Right, Speed = 5 }));
            //ls.AddToEnd(new RandomColorPerKeyEffect() { Brightness=10});
            //ls.AddToEnd(new BreathingColorEffect(new() { LedKey.B, LedKey.R, LedKey.E, LedKey.A, LedKey.T, LedKey.H, LedKey.I, LedKey.N, LedKey.G, }, Color.FromArgb(100,255,30)));
            ls.Speed = 5;

            lightLoop = Task.Run(ls.UpdateLoop);

            manager.AddHotKey(new[] { LedKey.P }, x => Console.WriteLine("Toggle"));
            manager.AddHotKey(new[] { LedKey.P, LedKey.L }, x => Console.WriteLine("Play"));
            manager.AddHotKey(new[] { LedKey.P, LedKey.P }, x => Console.WriteLine("Pause"));
            manager.AddHotKey(new[] { LedKey.T, LedKey.W }, x => OpenUrl("https://www.twitch.tv/"));
            manager.AddHotKey(new[] { LedKey.T, LedKey.W, LedKey.N }, x => OpenUrl("https://www.twitch.tv/noobdevtv"));

            foreach (var color in typeof(Color).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.PropertyType == typeof(Color)))
            {
                var hotkey = new List<LedKey> { LedKey.C, LedKey.O, LedKey.L };

                foreach (var key in color.Name.ToUpper())
                {
                    hotkey.Add(Enum.Parse<LedKey>(key.ToString()));
                }
                manager.AddHotKey(hotkey, (vk) =>
                {
                    var solid = ls.LightLayers.FirstOrDefault(x => x.GetType() == typeof(SolidColorEffect));
                    if (solid == default)
                        return;
                    ((SolidColorEffect)solid).SolidColor = (Color)(color.GetValue(null) ?? Color.Black);
                });
            }

            manager.AddHotKey(new[] { LedKey.C, LedKey.O, LedKey.L, LedKey.E, LedKey.M, LedKey.P, LedKey.T, LedKey.Y }, (vk) =>
            {
                var solid = ls.LightLayers.FirstOrDefault(x => x.GetType() == typeof(SolidColorEffect));
                if (solid == default)
                    return;
                ((SolidColorEffect)solid).SolidColor = null;
            });

            keyboard.VolumeKnobFxPressedReceived += (s, e) =>
            {
                ls.Brightness = (byte)(e.Data - 1);
            };

            string url = string.Empty;
            while (true)
            {
                var commands = Console.ReadLine()?.Split('|') ?? new[] { "" };
                var command = commands[0];

                switch (command)
                {
                    case "":
                        break;

                    case "newUrl":
                        CreateNewUrlHotKey(keyboard, manager, commands.Skip(1).ToList());
                        break;

                    case "record":
                        Console.WriteLine(string.Join(" -> ", manager.RecordKeys()));
                        break;

                    case "tooltips":
                        foreach (var item in Toolbar.GetToolbarButtonsText(Toolbar.ToolbarButtonLocation.All))
                            Console.WriteLine(item);
                        break;

                    default:
                        break;
                }
            }

        }

        private static void CreateNewUrlHotKey(VulcanKeyboard keyboard, HotKeyManager manager, IReadOnlyList<string> parameters)
        {
            var keys = new List<LedKey>();

            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;
            keyboard.VolumeKnobTurnedReceived += Keyboard_VolumeKnobTurnedReceived;
            void Keyboard_KeyPressedReceived(object? sender, KeyPressedArgs e)
            {
                if (e.IsPressed)
                    keys.Add(e.Key);
            }
            void Keyboard_VolumeKnobTurnedReceived(object? sender, VolumeKnDirectionArgs e)
            {
                if (e.TurnedRight)
                {
                    keyboard.KeyPressedReceived -= Keyboard_KeyPressedReceived;
                    keyboard.VolumeKnobTurnedReceived -= Keyboard_VolumeKnobTurnedReceived;
                    manager.AddHotKey(keys, k => OpenUrl(parameters[0]));
                }
            }
        }
    }
}