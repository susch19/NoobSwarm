using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool BlockInput([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool fBlockIt);

        private static readonly int[] ProductIds = new int[] { 0x307A, 0x3098 };

        private static Tree tree = new Tree();
        private static KeyNode currentNode;
        private static byte[] lastColorCopy;
        private static Random r = new Random();


        static void Main(string[] args)
        {
            //var scanner = new DeviceScanner(0x1E7D, 0x3098);
            //scanner.StartAsyncScan();

            //var dev = new USBDevice(0x1E7D, 0x3098, null);
            ////scanner.DeviceArrived += (o, s) => {; };
            ////scanner.DeviceRemoved += (o, s) => {; };
            //dev.InputReportArrivedEvent += Dev_InputReportArrivedEvent;
            //dev.StartAsyncRead();
            //Console.ReadLine();
            //scanner.StopAsyncScan();
            AutoResetEvent are = new AutoResetEvent(true);


            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.M, LedKey.O, LedKey.OEMPERIOD }, (vk) => { Console.WriteLine("Jay"); });
            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.M, LedKey.P, LedKey.OEMPERIOD }, (vk) => { Console.WriteLine("Jay2"); });
            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.A, LedKey.P, LedKey.OEMPERIOD }, (vk) => { Console.WriteLine("Jay123"); });

            tree.CreateNode(new List<LedKey> { LedKey.LEFT_CONTROL, LedKey.C }, (vk) => { Console.WriteLine("Copy"); });
            tree.CreateNode(new List<LedKey> { LedKey.LEFT_CONTROL, LedKey.K }, (vk) => { Console.WriteLine("KD"); });

            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.M, LedKey.O, LedKey.OEMPERIOD }, (vk) => { Console.WriteLine("JayCtrl"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.M, LedKey.P, LedKey.OEMPERIOD }, (vk) => { Console.WriteLine("JayCtrl2"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.A, LedKey.P, LedKey.OEMPERIOD }, (vk) => { Console.WriteLine("JayCtrl123"); });

            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.FN_Key, LedKey.RIGHT_CONTROL, LedKey.FN_Key }, (vk) => { Console.WriteLine("What?"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.FN_Key }, (vk) => { Console.WriteLine("What? Early exit"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.FN_Key, LedKey.R }, (vk) => { Console.WriteLine("Later Early Exit"); });

            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.FN_Key, LedKey.RIGHT_CONTROL, LedKey.LEFT_ALT }, (vk) => { Console.WriteLine("What??"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.RIGHT_CONTROL }, (vk) => { Console.WriteLine("Exit Virtual Box Capture"); });

            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.B, LedKey.A, LedKey.N, LedKey.A, LedKey.N, LedKey.A, LedKey.S }, (vk) => { Console.WriteLine("Bananas"); });

            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.B, LedKey.A, LedKey.N, LedKey.A, LedKey.N, LedKey.E, LedKey.N, LedKey.B, LedKey.R, LedKey.O, LedKey.T }, (vk) => { Console.WriteLine("Backt eh niemand"); });

            tree.CreateNode(new List<LedKey> { LedKey.Q, LedKey.Q }, (vk) =>
            {
                Console.WriteLine("New Random Colors");
                SetRandomKeyColors(vk);
                are.Set();
            });
            tree.CreateNode(new List<LedKey> { LedKey.Q, LedKey.W }, (vk) => { Console.WriteLine("Throw Granade"); });
            tree.CreateNode(new List<LedKey> { LedKey.Q, LedKey.E }, (vk) => { Console.WriteLine("Dont even know"); });


            foreach (var color in typeof(Color).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x=>x.PropertyType == typeof(Color)))
            {
                var hotkey = new List<LedKey> { LedKey.FN_Key};

                foreach (var key in color.Name.ToUpper())
                {
                    hotkey.Add(Enum.Parse<LedKey>(key.ToString()));
                }

                tree.CreateNode(hotkey, (vk) => {
                    Console.WriteLine("Set Color to: "+color.Name);
                    vk.SetColor((Color)color.GetValue(null));
                    are.Set();
                });
            }


            VulcanKeyboard keyboard = VulcanKeyboard.Initialize();
            SetRandomKeyColors(keyboard);

            keyboard.VolumeKnobTurnedReceived += (s, e) =>
            {
                //Console.WriteLine("Knob reached limit; Turned to direction " + (e.TurnedRight ? "Right" : "Left")); 
            };
            keyboard.VolumeKnobFxPressedReceived += (s, e) =>
            {
                //Console.WriteLine("FX Knob change value to " + (e.Data)); 
                if (s is VulcanKeyboard vk)
                {
                    vk.SetColor(Color.FromArgb((255 / 69) * e.Data, (255 / 69) * e.Data, (255 / 69) * e.Data));
                    are.Set();
                }

            };
            keyboard.VolumeKnobPressedReceived += (s, e) =>
            {
                //Console.WriteLine("Volume knob \"clicked\" " + e.IsPressed); 
            };
            keyboard.KeyPressedReceived += (s, e) =>
            {
                if (!(s is VulcanKeyboard vk))
                    return;

                currentNode ??= tree;
                if (!e.IsPressed)
                {
                    if (e.Key == LedKey.ESC && currentNode != tree)
                    {
                        currentNode = tree;
                        vk.SetColors(lastColorCopy);
                        are.Set();
                    }
                }

                if (e.IsPressed)
                {
                    if (e.Key == LedKey.ENTER && currentNode.KeineAhnungAction != null)
                    {
                        vk.SetColors(lastColorCopy);
                        currentNode.KeineAhnungAction(vk);
                        currentNode = tree;
                    }
                    else if (!currentNode.Children.TryGetValue(e.Key, out var node) && currentNode != tree)
                    {
                        vk.SetKeyColor(e.Key, Color.Red);
                        are.Set();
                        return;
                    }

                    else if (node is not null)
                    {
                        if (currentNode == tree)
                        {
                            lastColorCopy = vk.GetLastSendColorsCopy();

                        }

                        if (node.KeineAhnungAction == null && !node.HasSinglePath)
                        {
                            currentNode = node;
                            vk.SetColor(Color.Black);
                            foreach (var item in currentNode.Children)
                                vk.SetKeyColor(item.Key, Color.Green);

                            are.Set();
                            return;
                        }
                        else if (node.KeineAhnungAction != null && node.Children.Count > 0)
                        {
                            currentNode = node;
                            vk.SetColor(Color.Black);
                            foreach (var item in currentNode.Children)
                                vk.SetKeyColor(item.Key, Color.Green);

                            vk.SetKeyColor(LedKey.ENTER, Color.BlueViolet);
                            are.Set();

                            return;

                        }
                        currentNode = tree;
                        vk.SetColors(lastColorCopy);
                        node.SinglePathChild.KeineAhnungAction(vk);
                    }
                }

                //Console.WriteLine($"Key Pressed {e.Key} {e.IsPressed}"  );

                //if (e.IsPressed)
                //    vk.SetKeyColor(e.Key, Color.Blue);
                //else
                //    vk.SetKeyColor(e.Key, Color.Black);
                //are.Set();
            };

            var t = Task.Run(() =>
            {
                while (true)
                {
                    keyboard.Update();
                    are.WaitOne();
                    Thread.Sleep(16);
                }
            });

            Console.ReadLine();
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
