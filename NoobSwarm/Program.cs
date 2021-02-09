using HidSharp.Reports.Input;

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
    /* Dictionary<string, byte[]> keys = new Dictionary<string, byte[]>() {
                    { "esc", new byte[]{0x03, 0x00, 0xFB, 0x11 } },
                    { "fn", new byte[]{ 0x03 ,0x00 ,0xFB, 0x77 } },
                    { "easyshift", new byte[]{ 0x03 ,0x00 ,0x0A, 0xFF } },
                    { "oemminus", new byte[]{ 0x03 ,0x00 , 0xFB, 0x5E } },
                    { "c", new byte[]{ 0x03 ,0x00 , 0xFB, 0x2E } },
                    { "h", new byte[]{ 0x03 ,0x00 , 0xFB, 0x3D } },
                    { "e", new byte[]{ 0x03 ,0x00 , 0xFB, 0x24 } },
                    { "s", new byte[]{ 0x03 ,0x00 , 0xFB, 0x25 } },
                    { "m", new byte[]{ 0x03 ,0x00 , 0xFB, 0x46 } },
                    { "i", new byte[]{ 0x03 ,0x00 , 0xFB, 0x43 } },
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

        static void Main(string[] args)
        {
            using VulcanKeyboard keyboard = VulcanKeyboard.Initialize();
            if (keyboard == null)
            {
                Console.WriteLine("Did not find vulcan!");
                Console.ReadLine();
                return;
            }


            bool initKeyboard = true;
            bool fnPressed = false;
            bool easyPressed = false;

            Dictionary<int, byte[]> mapping = new Dictionary<int, byte[]>();
            Dictionary<int, Color> alreadyClickedKeys = new Dictionary<int, Color>();
            if (initKeyboard)
            {
                var are = new AutoResetEvent(false);
                var are2 = new AutoResetEvent(false);
                keyboard.SetColor(Color.Black);
                keyboard.SetKeyColor(0, Color.Green);
                keyboard.Update();

                int i = 0;
                Dictionary<string, string> keys = new Dictionary<string, string>() {
                    { "esc", "03 00 FB 11" },
                    { "fn", "03 00 FB 77" },
                    { "easyshift", "03 00 0A FF" },
                    { "oemminus", "03 00 FB 5E" },
                    { "c", "03 00 FB 2E" },
                    { "h", "03 00 FB 3D" },
                    { "e", "03 00 FB 24" },
                    { "s", "03 00 FB 25" },
                    { "m", "03 00 FB 46" },
                    { "i", "03 00 FB 43" },
                };
                Dictionary<string, string> keysReversed =
                    keys
                    .GroupBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.First().Key);

                var hotKey = new Dictionary<string, List<string>>()
                {
                    {"fn", new List<string>{"chess","chemie" } },
                };

                ByteEventArgs byteArgs = null;
                string s = string.Empty;

#pragma warning disable HAA0201 // Implicit string concatenation allocation
                keyboard.KeyPressedReceived += (object sender, ByteEventArgs even) =>
                {
                    byteArgs = even;
                    are2.Set();
                    try
                    {
                        var keyBytes = string.Join(' ', byteArgs.Bytes.Take(4).Select(x => x.ToString("X2")));

                        if (keyBytes == keys["fn"])
                        {
                            var htks = hotKey["fn"].ToList();
                            if (htks.Count > 1)
                            {
                                keyboard.SetColor(Color.Black);
                                foreach (var htk in htks)
                                {
                                    if (Enum.TryParse<Key>(htk[0].ToString().ToUpper(), out var key))
                                        keyboard.SetKeyColor(key, Color.Green);
                                }
                            }
                            keyboard.Update();
                            return;
                        }
                        if (keyBytes == keys["easyshift"])
                            return;

                        if (fnPressed && keyBytes == keys["oemminus"] && byteArgs.Bytes[4] > 0)
                        {
                            Console.WriteLine("You have made a custom fn hotkey :)");
                        }
                        if (fnPressed && keysReversed.TryGetValue(keyBytes, out var str))
                        {
                            var htks = hotKey["fn"].Where(x => x.Length > s.Length && x[s.Length].ToString() == str).ToList();
                            if (htks.Count > 1)
                            {
                                s += str;
                                keyboard.SetColor(Color.Black);
                                foreach (var htk in htks)
                                {
                                    if (Enum.TryParse<Key>(htk[s.Length].ToString().ToUpper(), out var key))
                                        keyboard.SetKeyColor(key, Color.Green);
                                }
                            }
                            else if (htks.Count == 1)
                            {
                                Console.WriteLine("Found hotkey: " + htks[0]);
                                keyboard.SetColor(Color.Green);
                            }
                            else
                            {
                                return;
                            }
                            keyboard.Update();
                            return;
                            ;
                        }

                        if (easyPressed && keyBytes == keys["oemminus"] && byteArgs.Bytes[4] > 0)
                        {
                            Console.WriteLine("You have made a custom easy hotkey :)");
                        }

                        if (keyBytes == keys["esc"])
                        {
                            i++;
                            keyboard.SetColor(Color.Black);
                            //keyboard.SetKeyColor(i, Color.Green);
                            keyboard.SetKeyColor(i / 2, Color.Green);
                            keyboard.Update();
                            return;
                        }

                        //i++;

                        //keyboard.SetColor(Color.Black);
                        ////keyboard.SetKeyColor(i, Color.Green);
                        //keyboard.SetKeyColor(i / 2, Color.Green);
                        keyboard.Update();

                        //mapping.Add(i, even.Bytes.Take(20).ToArray());

                        //if (i > 300)
                        //    are.Set();

                    }
                    catch (Exception e)
                    {

                    }
                };
#pragma warning restore HAA0201 // Implicit string concatenation allocation

                var task = Task.Run(() =>
                {
                    while (true)
                    {
                        are2.WaitOne();
                        var keyBytes = string.Join(' ', byteArgs.Bytes.Take(4).Select(x => x.ToString("X2")));
                        if (keyBytes == keys["fn"])
                        {
                            var block = byteArgs.Bytes[4] > 0;
                            if (!block)
                                s = string.Empty;
                            //keyboard.SetColor(Color.Red);
                            //else
                            //{
                            //    //keyboard.SetColor(Color.Green);
                            //}
                            //keyboard.Update();
                            fnPressed = block;
                            try
                            {
                                BlockInput(block);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                BlockInput(false);
                            }
                        }
                        else if (keyBytes == keys["easyshift"])
                        {
                            var block = byteArgs.Bytes[4] < 1;
                            if (block)
                                keyboard.SetColor(Color.Yellow);
                            else
                                keyboard.SetColor(Color.Green);
                            keyboard.Update();
                            easyPressed = block;
                            try
                            {
                                BlockInput(block);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                BlockInput(false);
                            }
                        }
                    }
                });
                are.WaitOne();


            }

        }
    }
}
