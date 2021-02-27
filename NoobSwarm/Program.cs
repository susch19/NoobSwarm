using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate void KeyboardTestCallback(int a, int b, int down);

        //[DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void StartHook();

        //[DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.StdCall)]
        //public static extern void SetCallback(IntPtr aCallback);
        //[DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.Cdecl)]

        //public static extern void start_message_loop();
        //[DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.Cdecl)]

        //public static extern void StopHook();

        private static readonly CancellationTokenSource cts = new();

        [STAThread]
        static void Main(string[] args)
        {
            Console.CancelKeyPress += (s, e) => cts.Cancel();

            //start_message_loop();

            //var t = Task.Run(MessageLoopCpp);
            //    .ContinueWith((a)=>StopHook());

            //SetCallback(Marshal.GetFunctionPointerForDelegate(del));

            //StartHook();

            //using var kb = new Keyboard();
            //kb.SendChar('h');
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            bool b = false;
            //while (true)
            //{
            //    var kc = Console.ReadKey().KeyChar;
            //    if (kc == 'w')
            //    {
            //        kb.SendVirtualKey((ushort)PInvoke.User32.VirtualKey.VK_TAB, Keyboard.KeyModifier.left_alt);
            //        Thread.Sleep(100);
            //        kb.SendVirtualKey((ushort)PInvoke.User32.VirtualKey.VK_LWIN);
            //        Thread.Sleep(100);
            //        kb.SendCharsSequene("Hallo, das hier ist ein kleiner Test :)");
            //        continue;
            //    }

            //    if (!b)
            //        kb.SendChar(kc);
            //    b = !b;
            //}
            //Func<ReportDescriptor, HidDeviceInputReceiver> generateHIDInputReceiver = (desc) =>
            //{
            //    return new HidDeviceInputReceiver(100, desc);
            //};
            //foreach (var device in devices)
            //{
            //    if (device.TryOpen(out var devStream))
            //    {
            //        var desc = device.GetReportDescriptor();
            //        var rec = generateHIDInputReceiver(desc);
            //        rec.Received += (s, e) =>
            //        {
            //            Console.WriteLine($"Received: Count: {e.Count}, Bytes: {string.Join(' ', e.Bytes.Skip(e.Offset).Take(e.Count).Select(x => x.ToString("X2")))}");

            //        };
            //        rec.Start(devStream);

            //    }
            //}
            //while (true)
            //    Console.ReadLine();

            using var keyboard = VulcanKeyboard.Initialize();
            var ls = new LightService(keyboard);
            var manager = new HotKeyManager(keyboard, ls, LedKey.FN_Key);
            var makroManager = new MakroManager();
            ls.AddToEnd(new HSVColorWanderEffect());
            //ls.AddToEnd(new HSVColorWanderEffect(Enum.GetValues<LedKey>().Where(x => x.ToString().Length == 1).ToList(), new List<Color> { Color.Orange, Color.Green, Color.Red }) { Direction = Direction.Up, Speed = 1 });
            //ls.AddToEnd(new HSVColorWanderEffect(Enum.GetValues<LedKey>().Where(x => x.ToString()[0] == 'F' && x.ToString().Length is <= 3 and > 1).ToList()) { Direction = Direction.Right });
            //ls.AddToEnd(new HSVColorWanderEffect(Enum.GetValues<LedKey>().Where(x => x.ToString()[0] == 'D' && x.ToString().Length == 2).ToList()) { Direction = Direction.Left });
            //ls.AddToEnd(new BreathingColorPerKeyEffect(Enum.GetValues<LedKey>().Where(x => (byte)x >= 113).ToList(), new HSVColorWanderEffect()) { Speed = .1f });
            //ls.AddToEnd(new SingleKeysColorEffect(new() { { LedKey.ESC, Color.White } }));
            ls.AddToEnd(new SolidColorEffect() {Brightness = 50});
            //ls.AddToEnd(new PressedFadeOutEffect(new HSVColorWanderEffect() { Direction = Direction.Right, Speed = 5 }));
            //ls.AddToEnd(new RandomColorPerKeyEffect() { Brightness=10});
            //ls.AddToEnd(new BreathingColorEffect(new() { LedKey.B, LedKey.R, LedKey.E, LedKey.A, LedKey.T, LedKey.H, LedKey.I, LedKey.N, LedKey.G, }, Color.FromArgb(100,255,30)));
            ls.Speed = 5;

            _ = Task.Run(() => ls.UpdateLoop(cts.Token));

            // manager.AddHotKey(new[] { LedKey.P }, x => Console.WriteLine("Toggle"));
            // manager.AddHotKey(new[] { LedKey.P, LedKey.L }, x => Console.WriteLine("Play"));
            // manager.AddHotKey(new[] { LedKey.P, LedKey.P }, x => Console.WriteLine("Pause"));
            // manager.AddHotKey(new[] { LedKey.T, LedKey.W }, x => OpenUrl("https://www.twitch.tv/"));
            // manager.AddHotKey(new[] { LedKey.T, LedKey.W, LedKey.N }, x => OpenUrl("https://www.twitch.tv/noobdevtv"));

            // foreach (var color in typeof(Color).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => x.PropertyType == typeof(Color)))
            // {
            //     var hotkey = new List<LedKey> { LedKey.C, LedKey.O, LedKey.L };
            //
            //     foreach (var key in color.Name.ToUpper())
            //     {
            //         hotkey.Add(Enum.Parse<LedKey>(key.ToString()));
            //     }
            //     manager.AddHotKey(hotkey, (vk) =>
            //     {
            //         var solid = ls.LightLayers.FirstOrDefault(x => x.GetType() == typeof(SolidColorEffect));
            //         if (solid == default)
            //             return;
            //         ((SolidColorEffect)solid).SolidColor = (Color)(color.GetValue(null) ?? Color.Black);
            //     });
            // }
            //
            // manager.AddHotKey(new[] { LedKey.C, LedKey.O, LedKey.L, LedKey.E, LedKey.M, LedKey.P, LedKey.T, LedKey.Y }, (vk) =>
            // {
            //     var solid = ls.LightLayers.FirstOrDefault(x => x.GetType() == typeof(SolidColorEffect));
            //     if (solid == default)
            //         return;
            //     ((SolidColorEffect)solid).SolidColor = null;
            // });

            keyboard.VolumeKnobFxPressedReceived += (s, e) => { ls.Brightness = (byte) (e.Data - 1); };

            string url = string.Empty;
            List<(LedKey, int)> lastRecorded = null;
            List<MakroManager.RecordKey> lastRecordedMM = null;
            while (true)
            {
                var commands = Console.ReadLine()?.Split('|') ?? new[] {""};
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

        private static void CreateNewUrlHotKey(VulcanKeyboard keyboard, HotKeyManager manager,
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