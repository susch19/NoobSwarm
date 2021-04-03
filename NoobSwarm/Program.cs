using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Makros;

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

        private static readonly CancellationTokenSource cts = new();

        [STAThread]
        static void Main(string[] args)
        {
            Console.CancelKeyPress += (s, e) => cts.Cancel();
            TypeContainer.Register<IVulcanKeyboard>(VulcanKeyboard.Initialize());
            TypeContainer.Register<LightService>(InstanceBehaviour.Singleton);
            TypeContainer.Register<HotKeyManager>(InstanceBehaviour.Singleton);


            //start_message_loop();

            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            bool b = false;



            var asdhotkey = new MakroHotkeyCommand(new List<MakroManager.RecordKey> {
                new MakroManager.RecordKey(Makros.Key.A, 120,true),
                new MakroManager.RecordKey(Makros.Key.A, 50,false)
            });



            var manager = TypeContainer.Get<HotKeyManager>();
            manager.Mode = HotKeyMode.Active;
            manager.AddHotKey(new List<LedKey> { LedKey.A }, asdhotkey);


            var ls = TypeContainer.Get<LightService>();
            var keyboard = TypeContainer.Get<IVulcanKeyboard>();
            //ls.AddToEnd(new HSVColorWanderEffect());

            //ls.AddToEnd(new SolidColorEffect() { Brightness = 50 });

            ls.Speed = 5;

            _ = Task.Run(() => ls.UpdateLoop(cts.Token));



            foreach (var color in typeof(Color)
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(x => x.PropertyType == typeof(Color)))
            {
                var hotkey = new List<LedKey> { LedKey.C, LedKey.O, LedKey.L };

                foreach (var key in color.Name.ToUpper())
                {
                    hotkey.Add(Enum.Parse<LedKey>(key.ToString()));
                }

                manager.AddHotKey(hotkey,
                    new AddRemoveLightCommand(
                        new Lights.LightEffects.Wrapper.LightEffectWrapper(
                        new SolidColorEffect((Color)(color.GetValue(null) ?? Color.Black)))));
            }


            keyboard.VolumeKnobFxPressedReceived += (s, e) => { ls.Brightness = (byte)(e.Data - 1); };

            string url = string.Empty;
            List<(LedKey, int)> lastRecorded = null;
            List<MakroManager.RecordKey> lastRecordedMM = null;
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
                        //var recorded = manager.RecordKeys().Select(x => x).ToList();
                        //if (recorded.Count < 1)
                        //    break;

                        //var makro = makroManager.RecordKeys().ToList();
                        //manager.AddHotKey(recorded, (vkb) => RunMakro(kb, makro));

                        //Console.WriteLine(string.Join(" -> ", recorded));
                        break;
                    case "recordWithTime":
                        //lastRecordedMM = makroManager.StartRecording().ToList();
                        //Console.WriteLine(string.Join("\r\n", lastRecordedMM));
                        break;
                    case "playback":
                        //if (lastRecordedMM is null)
                        //    break;
                        //RunMakro(kb, lastRecordedMM);
                        break;

                    default:
                        break;
                }
            }
        }

        //private static void MessageLoopCpp()
        //{
        //    var del = new KeyboardTestCallback((a, b, d) =>
        //    {
        //        Console.WriteLine($"Received Key {(PInvoke.User32.VirtualKey)a}, {b}, {(d == 256 ? "Down": "Up")}");
        //    });
        //    StartHook();
        //    SetCallback(Marshal.GetFunctionPointerForDelegate(del));
        //    start_message_loop();
        //}

        //private static void RunMakro(Keyboard kb, List<MakroManager.RecordKey> lastRecordedMM)
        //{
        //    Keyboard.KeyModifier modifier = Keyboard.KeyModifier.None;
        //    Task.Run(() =>
        //    {
        //        for (int i = 0; i < lastRecordedMM.Count; i++)
        //        {
        //            var record = lastRecordedMM[i];
        //            var item = record.Key;
        //            var keyStr = item.ToString();
        //            Thread.Sleep(record.TimeBeforePress);
        //            if (record.Pressed)
        //            {
        //                if (Enum.TryParse<PInvoke.User32.VirtualKey>("VK_" + keyStr, out var virtualKey))
        //                    kb.SendVirtualKey((ushort)virtualKey, modifier);
        //                else if (char.TryParse(keyStr.ToLower(), out char res))
        //                    kb.SendChar(res, modifier);
        //                else if (keyStr.Length == 2 && keyStr[0] == 'D')
        //                    kb.SendChar(keyStr[1], modifier);
        //                else if (item == LedKey.SPACE)
        //                    kb.SendChar(' ', modifier);
        //                else if (item == LedKey.ENTER)
        //                    kb.SendVirtualKey(13, modifier);
        //                else if (item == LedKey.TAB)
        //                    kb.SendVirtualKey((ushort)PInvoke.User32.VirtualKey.VK_TAB, modifier);
        //                else if (item == LedKey.OEMPERIOD)
        //                    kb.SendVirtualKey((ushort)PInvoke.User32.VirtualKey.VK_OEM_PERIOD, modifier);
        //                else if (item == LedKey.OEMMINUS)
        //                    kb.SendVirtualKey((ushort)PInvoke.User32.VirtualKey.VK_OEM_MINUS, modifier);
        //                else if (item == LedKey.OEMCOMMA)
        //                    kb.SendVirtualKey((ushort)PInvoke.User32.VirtualKey.VK_OEM_COMMA, modifier);

        //            }

        //            if (item == LedKey.LEFT_SHIFT || item == LedKey.RIGHT_SHIFT)
        //            {
        //                var mod = Keyboard.KeyModifier.left_shift;
        //                if (record.Pressed)
        //                    modifier |= mod;
        //                else if ((modifier & mod) > 0)
        //                    modifier -= mod;
        //            }
        //            else if (item == LedKey.LEFT_ALT || item == LedKey.RIGHT_ALT)
        //            {
        //                var mod = Keyboard.KeyModifier.left_alt;
        //                if (record.Pressed)
        //                    modifier |= mod;
        //                else if ((modifier & mod) > 0)
        //                    modifier -= mod;
        //            }
        //            else if (item == LedKey.LEFT_CONTROL || item == LedKey.RIGHT_CONTROL)
        //            {
        //                var mod = Keyboard.KeyModifier.left_control;
        //                if (record.Pressed)
        //                    modifier |= mod;
        //                else if ((modifier & mod) > 0)
        //                    modifier -= mod;
        //            }
        //            else if (item == LedKey.LEFT_WINDOWS)
        //            {
        //                if (lastRecordedMM.Count > i && lastRecordedMM[i + 1].Key == LedKey.LEFT_WINDOWS)
        //                {
        //                    kb.SendVirtualKey(0, Keyboard.KeyModifier.left_gui);
        //                    i++;
        //                }
        //                else
        //                {
        //                    var mod = Keyboard.KeyModifier.left_gui;
        //                    if (record.Pressed)
        //                        modifier |= mod;
        //                    else if ((modifier & mod) > 0)
        //                        modifier -= mod;
        //                }
        //            }

        //        }
        //    });
        //}

        private static void CreateNewUrlHotKey(IVulcanKeyboard keyboard, HotKeyManager manager,
            IReadOnlyList<string> parameters)
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
                    // manager.AddHotKey(keys, k => OpenUrl(parameters[0]));
                }
            }
        }
    }
}