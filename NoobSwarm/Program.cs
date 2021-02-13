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


            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.M, LedKey.O, LedKey.OEMPERIOD }, () => { Console.WriteLine("Jay"); });
            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.M, LedKey.P, LedKey.OEMPERIOD }, () => { Console.WriteLine("Jay2"); });
            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.A, LedKey.P, LedKey.OEMPERIOD }, () => { Console.WriteLine("Jay123"); });

            tree.CreateNode(new List<LedKey> { LedKey.LEFT_CONTROL, LedKey.C }, () => { Console.WriteLine("Copy"); });
            tree.CreateNode(new List<LedKey> { LedKey.LEFT_CONTROL, LedKey.K }, () => { Console.WriteLine("KD"); });

            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.M, LedKey.O, LedKey.OEMPERIOD }, () => { Console.WriteLine("JayCtrl"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.M, LedKey.P, LedKey.OEMPERIOD }, () => { Console.WriteLine("JayCtrl2"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.A, LedKey.P, LedKey.OEMPERIOD }, () => { Console.WriteLine("JayCtrl123"); });

            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.FN_Key, LedKey.RIGHT_CONTROL, LedKey.FN_Key }, () => { Console.WriteLine("What?"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.FN_Key }, () => { Console.WriteLine("What? Early exit"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.FN_Key, LedKey.R }, () => { Console.WriteLine("Later Early Exit"); });

            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.FN_Key, LedKey.RIGHT_CONTROL, LedKey.LEFT_ALT }, () => { Console.WriteLine("What??"); });
            tree.CreateNode(new List<LedKey> { LedKey.RIGHT_CONTROL, LedKey.RIGHT_CONTROL }, () => { Console.WriteLine("Exit Virtual Box Capture"); });

            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.B, LedKey.A, LedKey.N, LedKey.A, LedKey.N, LedKey.A, LedKey.S }, () => { Console.WriteLine("Bananas"); });

            tree.CreateNode(new List<LedKey> { LedKey.FN_Key, LedKey.B, LedKey.A, LedKey.N, LedKey.A, LedKey.N, LedKey.E, LedKey.N, LedKey.B, LedKey.R, LedKey.O, LedKey.T }, () => { Console.WriteLine("Backt eh niemand"); });

            tree.CreateNode(new List<LedKey> { LedKey.Q, LedKey.Q }, () => { Console.WriteLine("Q"); });
            tree.CreateNode(new List<LedKey> { LedKey.Q, LedKey.W }, () => { Console.WriteLine("Throw Granade"); });
            tree.CreateNode(new List<LedKey> { LedKey.Q, LedKey.E }, () => { Console.WriteLine("Dont even know"); });

            AutoResetEvent are = new AutoResetEvent(true);
            using VulcanKeyboard keyboard = VulcanKeyboard.Initialize();
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
                        vk.SetColor(Color.Blue);
                        are.Set();
                    }
                }

                if (e.IsPressed)
                {
                    if (e.Key == LedKey.ENTER && currentNode.KeineAhnungAction != null)
                    {
                        currentNode.KeineAhnungAction();
                        currentNode = tree;
                        vk.SetColor(Color.Yellow);
                    }
                    else if (!currentNode.Children.TryGetValue(e.Key, out var node) && currentNode != tree)
                    {
                        vk.SetKeyColor(e.Key, Color.Red);
                        are.Set();
                        return;
                    }

                    else if (node is not null)
                    {
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
                        node.SinglePathChild.KeineAhnungAction();
                        currentNode = tree;
                        vk.SetColor(Color.Yellow);
                    }
                }

                //Console.WriteLine($"Key Pressed {e.Key} {e.IsPressed}"  );

                if (e.IsPressed)
                    vk.SetKeyColor(e.Key, Color.Blue);
                else
                    vk.SetKeyColor(e.Key, Color.Black);
                are.Set();
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

            BlockInput(true);

            Thread.Sleep(10000);
            BlockInput(false);

            //            if (keyboard == null)
            //            {
            //                Console.WriteLine("Did not find vulcan!");
            //                Console.ReadLine();
            //                return;
            //            }


            //            bool initKeyboard = true;
            //            bool fnPressed = false;
            //            bool easyPressed = false;

            //            Dictionary<int, byte[]> mapping = new Dictionary<int, byte[]>();
            //            Dictionary<int, Color> alreadyClickedKeys = new Dictionary<int, Color>();
            //            if (initKeyboard)
            //            {
            //                var are = new AutoResetEvent(false);
            //                var are2 = new AutoResetEvent(false);
            //                keyboard.SetColor(Color.Black);
            //                keyboard.SetKeyColor(0, Color.Green);
            //                //keyboard.Update();

            //                int i = 0;
            //                Dictionary<string, string> keys = new Dictionary<string, string>() {
            //                    { "esc", "03 00 FB 11" },
            //                    { "fn", "03 00 FB 77" },
            //                    { "easyshift", "03 00 0A FF" },
            //                    { "oemminus", "03 00 FB 5E" },
            //                    { "c", "03 00 FB 2E" },
            //                    { "h", "03 00 FB 3D" },
            //                    { "e", "03 00 FB 24" },
            //                    { "s", "03 00 FB 25" },
            //                    { "m", "03 00 FB 46" },
            //                    { "i", "03 00 FB 43" },
            //                };
            //                Dictionary<string, string> keysReversed =
            //                    keys
            //                    .GroupBy(x => x.Value)
            //                    .ToDictionary(x => x.Key, x => x.First().Key);

            //                var hotKey = new Dictionary<string, List<string>>()
            //                {
            //                    {"fn", new List<string>{"chess","chemie" } },
            //                };

            //                ByteEventArgs byteArgs = null;
            //                string s = string.Empty;

            //#pragma warning disable HAA0201 // Implicit string concatenation allocation
            //                keyboard.KeyPressedReceived += (object sender, ByteEventArgs even) =>
            //                {
            //                    byteArgs = even;
            //                    are2.Set();
            //                    try
            //                    {
            //                        var keyBytes = string.Join(' ', byteArgs.Bytes.Take(4).Select(x => x.ToString("X2")));

            //                        if (keyBytes == keys["fn"])
            //                        {
            //                            var htks = hotKey["fn"].ToList();
            //                            if (htks.Count > 1)
            //                            {
            //                                keyboard.SetColor(Color.Black);
            //                                foreach (var htk in htks)
            //                                {
            //                                    if (Enum.TryParse<Key>(htk[0].ToString().ToUpper(), out var key))
            //                                        keyboard.SetKeyColor(key, Color.Green);
            //                                }
            //                            }
            //                            //keyboard.Update();
            //                            return;
            //                        }
            //                        if (keyBytes == keys["easyshift"])
            //                            return;

            //                        if (fnPressed && keyBytes == keys["oemminus"] && byteArgs.Bytes[4] > 0)
            //                        {
            //                            Console.WriteLine("You have made a custom fn hotkey :)");
            //                        }
            //                        if (fnPressed && keysReversed.TryGetValue(keyBytes, out var str))
            //                        {
            //                            var htks = hotKey["fn"].Where(x => x.Length > s.Length && x[s.Length].ToString() == str).ToList();
            //                            if (htks.Count > 1)
            //                            {
            //                                s += str;
            //                                keyboard.SetColor(Color.Black);
            //                                foreach (var htk in htks)
            //                                {
            //                                    if (Enum.TryParse<Key>(htk[s.Length].ToString().ToUpper(), out var key))
            //                                        keyboard.SetKeyColor(key, Color.Green);
            //                                }
            //                            }
            //                            else if (htks.Count == 1)
            //                            {
            //                                Console.WriteLine("Found hotkey: " + htks[0]);
            //                                keyboard.SetColor(Color.Green);
            //                            }
            //                            else
            //                            {
            //                                return;
            //                            }
            //                            //keyboard.Update();
            //                            return;
            //                            ;
            //                        }

            //                        if (easyPressed && keyBytes == keys["oemminus"] && byteArgs.Bytes[4] > 0)
            //                        {
            //                            Console.WriteLine("You have made a custom easy hotkey :)");
            //                        }

            //                        if (keyBytes == keys["esc"])
            //                        {
            //                            i++;
            //                            keyboard.SetColor(Color.Black);
            //                            //keyboard.SetKeyColor(i, Color.Green);
            //                            keyboard.SetKeyColor(i / 2, Color.Green);
            //                            //keyboard.Update();
            //                            return;
            //                        }

            //                        //i++;

            //                        //keyboard.SetColor(Color.Black);
            //                        ////keyboard.SetKeyColor(i, Color.Green);
            //                        //keyboard.SetKeyColor(i / 2, Color.Green);
            //                        //keyboard.Update();

            //                        //mapping.Add(i, even.Bytes.Take(20).ToArray());

            //                        //if (i > 300)
            //                        //    are.Set();

            //                    }
            //                    catch (Exception e)
            //                    {

            //                    }
            //                };
            //#pragma warning restore HAA0201 // Implicit string concatenation allocation

            //                var task = Task.Run(() =>
            //                {
            //                    while (true)
            //                    {
            //                        are2.WaitOne();
            //                        var keyBytes = string.Join(' ', byteArgs.Bytes.Take(4).Select(x => x.ToString("X2")));
            //                        if (keyBytes == keys["fn"])
            //                        {
            //                            var block = byteArgs.Bytes[4] > 0;
            //                            if (!block)
            //                                s = string.Empty;
            //                            //keyboard.SetColor(Color.Red);
            //                            //else
            //                            //{
            //                            //    //keyboard.SetColor(Color.Green);
            //                            //}
            //                            //keyboard.Update();
            //                            fnPressed = block;
            //                            try
            //                            {
            //                                BlockInput(block);
            //                            }
            //                            catch (Exception e)
            //                            {
            //                                Console.WriteLine(e.Message);
            //                                BlockInput(false);
            //                            }
            //                        }
            //                        else if (keyBytes == keys["easyshift"])
            //                        {
            //                            var block = byteArgs.Bytes[4] < 1;
            //                            if (block)
            //                                keyboard.SetColor(Color.Yellow);
            //                            else
            //                                keyboard.SetColor(Color.Green);
            //                            //keyboard.Update();
            //                            easyPressed = block;
            //                            try
            //                            {
            //                                BlockInput(block);
            //                            }
            //                            catch (Exception e)
            //                            {
            //                                Console.WriteLine(e.Message);
            //                                BlockInput(false);
            //                            }
            //                        }
            //                    }
            //                });
            //                are.WaitOne();


            //            }
            Console.ReadLine();

        }
    }
}
