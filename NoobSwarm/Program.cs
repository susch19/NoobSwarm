using NoobSwarm.Brushes;
using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm
{
    
    class Program
    {
      

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
            ls.AddToEnd(new RGBWanderEffect());
            ls.AddToEnd(new SingleKeysColorEffect(new() { { LedKey.ESC, Color.White } }));
            //ls.AddToEnd(new SolidColorEffect());
            //ls.AddToEnd(new RandomColorPerKeyEffect());
            ls.AddToEnd(new BreathingColorEffect(new() { LedKey.B, LedKey.R, LedKey.E, LedKey.A, LedKey.T, LedKey.H, LedKey.I, LedKey.N, LedKey.G, }, Color.FromArgb(100,255,30)));
            ls.Speed = 5;


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
                    ((SolidColorEffect)solid).SolidColor = (Color)color.GetValue(null);
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
                var commands = Console.ReadLine().Split('|');
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
            void Keyboard_KeyPressedReceived(object sender, KeyPressedArgs e)
            {
                if (e.IsPressed)
                    keys.Add(e.Key);
            }
            void Keyboard_VolumeKnobTurnedReceived(object sender, VolumeKnDirectionArgs e)
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