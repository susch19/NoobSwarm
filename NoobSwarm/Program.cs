using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm
{
    /* Dictionary<string, byte> keys = new Dictionary<string, byte>() {
                    { "esc", 0x11  },
                    { "fn", 0x77  },
                    { "easyshift", new byte[]{ 0x03 ,0x00 ,0x0A, 0xFF } },
                    { "oemminus",0x5E  },
                    { "c", 0x2E  },
                    { "h", 0x3D  },
                    { "e", 0x24  },
                    { "s", 0x25  },
                    { "m", 0x46  },
                    { "i", 0x43  },
                };
                Dictionary<byte[], string> keysReversed = 
                    keys
                    .GroupBy(x=>x.Value)
                    .ToDictionary(x=>x.Key, x=>x.First().Key);

                var hotKey = new Dictionary<string, List<string>>()
                {
                    {"fn", new List<string>{"chess","chemie" } },
                };*/
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        private static readonly int[] ProductIds = new int[] { 0x307A, 0x3098 };

        private static Tree tree = new Tree();
        private static KeyNode currentNode;
        private static byte[] lastColorCopy;
        private static Random r = new Random();

        private static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }

        static void Main(string[] args)
        {

            AutoResetEvent are = new AutoResetEvent(true);


            using var keyboard = VulcanKeyboard.Initialize();

            var manager = new HotKeyManager(keyboard, LedKey.FN_Key);



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
                    Console.WriteLine("Set Color to: " + color.Name);
                    vk.SetColor((Color)color.GetValue(null));
                    vk.Update();
                });
            }



            keyboard.VolumeKnobFxPressedReceived += (s, e) =>
            {
                //Console.WriteLine("FX Knob change value to " + (e.Data)); 
                if (s is VulcanKeyboard vk)
                {
                    vk.Brightness = (byte)(e.Data - 1);
                    vk.Update();
                }
            };
            //keyboard.VolumeKnobPressedReceived += (s, e) =>
            //{
            //    //Console.WriteLine("Volume knob \"clicked\" " + e.IsPressed); 
            //};
            //keyboard.KeyPressedReceived += (s, e) =>
            //{
            //    if (!(s is VulcanKeyboard vk))
            //        return;

            //    currentNode ??= tree;
            //    if (!e.IsPressed)
            //    {
            //        if (e.Key == LedKey.ESC && currentNode != tree)
            //        {
            //            currentNode = tree;
            //            vk.SetColors(lastColorCopy);
            //            are.Set();
            //        }
            //    }

            //    if (e.IsPressed)
            //    {
            //        if (e.Key == LedKey.ENTER && currentNode.KeineAhnungAction != null)
            //        {
            //            vk.SetColors(lastColorCopy);
            //            currentNode.KeineAhnungAction(vk);
            //            currentNode = tree;
            //        }
            //        else if (!currentNode.Children.TryGetValue(e.Key, out var node) && currentNode != tree)
            //        {
            //            vk.SetKeyColor(e.Key, Color.Red);
            //            are.Set();
            //            return;
            //        }

            //        else if (node is not null)
            //        {
            //            if (currentNode == tree)
            //            {
            //                lastColorCopy = vk.GetLastSendColorsCopy();

            //            }

            //            if (node.KeineAhnungAction == null && !node.HasSinglePath)
            //            {
            //                currentNode = node;
            //                vk.SetColor(Color.Black);
            //                foreach (var item in currentNode.Children)
            //                    vk.SetKeyColor(item.Key, Color.Green);

            //                are.Set();
            //                return;
            //            }
            //            else if (node.KeineAhnungAction != null && node.Children.Count > 0)
            //            {
            //                currentNode = node;
            //                vk.SetColor(Color.Black);
            //                foreach (var item in currentNode.Children)
            //                    vk.SetKeyColor(item.Key, Color.Green);

            //                vk.SetKeyColor(LedKey.ENTER, Color.BlueViolet);
            //                are.Set();

            //                return;

            //            }
            //            currentNode = tree;
            //            vk.SetColors(lastColorCopy);
            //            node.SinglePathChild.KeineAhnungAction(vk);
            //        }
            //    }

            //    //Console.WriteLine($"Key Pressed {e.Key} {e.IsPressed}"  );

            //    //if (e.IsPressed)
            //    //    vk.SetKeyColor(e.Key, Color.Blue);
            //    //else
            //    //    vk.SetKeyColor(e.Key, Color.Black);
            //    //are.Set();
            //};

            //var t = Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        await keyboard.Update();
            //        are.WaitOne();

            //        Thread.Sleep(16);
            //    }
            //});
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



        private static void SetRandomKeyColors(VulcanKeyboard keyboard)
        {
            for (int i = 0; i < 131; i++)
            {
                keyboard.SetKeyColor(i, Color.FromArgb(255 << 24 | r.Next(0, 255 << 16)));
            }
        }
    }
}
