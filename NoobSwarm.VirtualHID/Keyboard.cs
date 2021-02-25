using NoobSwarm.Hid;
using NoobSwarm.Hid.Reports;

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace NoobSwarm.VirtualHID
{
    public class Keyboard : IDisposable
    {
        private bool disposedValue;
        private IntPtr keyboardLayout;
        private HidStream vKeyboardStream;
        private readonly byte[] buf;
        private const int shift = 0b100000000;
        private const int ctrl = 0b1000000000;
        private const int alt = 0b10000000000;

        [Flags]
        public enum KeyModifier : byte
        {
            None = 0,
            left_control = 1 << 0,
            left_shift = 1 << 1,
            left_alt = 1 << 2,
            left_gui = 1 << 3,
            right_control = 1 << 4,
            right_shift = 1 << 5,
            right_alt = 1 << 6,
            right_gui = 1 << 7
        }


        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

        public Keyboard()
        {
            keyboardLayout = GetKeyboardLayout(0);
            var devices = DeviceList.Local.GetHidDevices(255).Where(x => x.ProductID == 0xAFFE).ToList();
            var controlDevice = devices.FirstOrDefault(x => x.GetReportDescriptor().Reports.Any(y => y.ReportID == 0x40));
            var kbDevice = devices.First(x => x.DevicePath.EndsWith("kbd"));

            if (controlDevice != default && controlDevice.TryOpen(out vKeyboardStream))
            {
                var rd = controlDevice.GetReportDescriptor();
                var rdkbd = kbDevice.GetReportDescriptor();
                var inputRep = rd.Reports.First(x => x.ReportType == ReportType.Input);
                var inputRepKbd = rdkbd.Reports.First(x => x.ReportType == ReportType.Input);
                buf = inputRep.CreateBuffer();
                buf[0] = 0x40;
                buf[1] = (byte)inputRepKbd.Length;//length of subReport
                //buf[2] = inputRepKbd.ReportID;
                //buf[3] = 8;
                //try
                //{
                //    //vKeyboardStream.Write(buf);
                //    //buf[3] = 0;
                //    //vKeyboardStream.Write(buf);
                //    //Thread.Sleep(10);
                //    buf[5] = 0x0;
                //    buf[6] = 0xcd;
                //    //buf[6] = 0x01;
                //    vKeyboardStream.Write(buf);
                //    Thread.Sleep(100);
                //    buf[5] = 0x0;
                //    buf[6] = 0x0;
                //    //buf[6] = 0x0;
                //    vKeyboardStream.Write(buf);
                //    //buf[5] = 0x08;
                //    //vKeyboardStream.Write(buf);
                //    //buf[5] = 0x0f;
                //    //vKeyboardStream.Write(buf);
                //    //buf[5] = 0x0f;
                //    //vKeyboardStream.Write(buf);
                //    //buf[5] = 0x12;
                //    //vKeyboardStream.Write(buf);
                //    //buf[5] = 0x1f;
                //    //vKeyboardStream.Write(buf);
                //    //buf[5] = 0x0;
                //    //vKeyboardStream.Write(buf);
                //    Thread.Sleep(5000);
                //}
                //finally
                //{
                //    buf[5] = 0x0;
                //    vKeyboardStream.Write(buf);
                //}


            }
        }

        //public string VKCodeToUnicode(uint VKCode)
        //{
        //    System.Text.StringBuilder sbString = new System.Text.StringBuilder();

        //    byte[] bKeyState = new byte[255];
        //    bool bKeyStateStatus = GetKeyboardState(bKeyState);
        //    if (!bKeyStateStatus)
        //        return "";
        //    uint lScanCode = MapVirtualKey(VKCode, 0);

        //    var shift = 0b100000000;
        //    var strg = 0b1000000000;
        //    var alt = 0b10000000000;

        //    var smallH = VkKeyScanEx('a', keyboardLayout);

        //    var mapped = PInvoke.User32.MapVirtualKey(smallH, PInvoke.User32.MapVirtualKeyTranslation.MAPVK_VK_TO_VSC);
        //    var smallZ = VkKeyScanEx('z', keyboardLayout);
        //    var mappedZ = PInvoke.User32.MapVirtualKey(smallZ, PInvoke.User32.MapVirtualKeyTranslation.MAPVK_VK_TO_VSC);
        //    var bigZ = VkKeyScanEx('Z', keyboardLayout);
        //    var smallA = VkKeyScanEx('a', keyboardLayout);
        //    var bigA = VkKeyScanEx('A', keyboardLayout);
        //    var small2 = VkKeyScanEx('2', keyboardLayout);
        //    var big2 = VkKeyScanEx('"', keyboardLayout);
        //    var huge2 = VkKeyScanEx('²', keyboardLayout);

        //    ToUnicodeEx(VKCode, lScanCode, bKeyState, sbString, (int)5, (uint)0, HKL);
        //    return sbString.ToString();
        //}

        public void SendVirtualKey(ushort key)
        {
            key = ExtractModifierAndWinScanCode(key, out KeyModifier modifier, out var winScanCode);

            if (winScanCode == 91)
            {
                modifier |= KeyModifier.left_gui;
                winScanCode -= 91;
            }
            if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
            {
                SendKey(modifier, val);
            }
            else if (modifier > 0 && val == 0)
            {
                SendKey(modifier, 0x070000);
            }
        }
        public void SendVirtualKey(ushort key, KeyModifier modifier)
        {

            key = ExtractModifierAndWinScanCode(key, out KeyModifier addModifier, out var winScanCode);

            modifier |= addModifier;
            if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
            {
                SendKey(modifier, val);
            }
            else if (modifier > 0 && val == 0)
            {
                SendKey(modifier, 0x070000);
            }
        }

        public void SendVirtualKeysSequence(ReadOnlySpan<ushort> keys, KeyModifier modifier)
        {

            foreach (var item in keys)
            {
                ExtractModifierAndWinScanCode(item, out KeyModifier addModifier, out var winScanCode);

                if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
                {
                    SendKey(modifier, val);
                }
                else if (modifier > 0 && val == 0)
                {
                    SendKey(modifier, 0x070000);
                }
            }

           
        }

        public void SendChar(char charToTest)
        {
            var virtualKey = (ushort)VkKeyScanEx(charToTest, keyboardLayout);
            KeyModifier modifier;
            ushort winScanCode;
            virtualKey = ExtractModifierAndWinScanCode(virtualKey, out modifier, out winScanCode);

            if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
            {
                SendKey(modifier, val);
            }
        }
        public void SendChar(char charToTest, KeyModifier modifier)
        {
            var virtualKey = (ushort)VkKeyScanEx(charToTest, keyboardLayout);

            virtualKey = ExtractModifierAndWinScanCode(virtualKey, out KeyModifier addmodifier, out var winScanCode);
            modifier |= addmodifier;

            if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
            {
                SendKey(modifier, val);
            }
        }
        public bool SendCharsSequene(ReadOnlySpan<char> charsToTest)
        {
            Span<byte> modifier = stackalloc byte[charsToTest.Length];
            Span<ushort> winScanCode = stackalloc ushort[charsToTest.Length];
            for (int i = 0; i < charsToTest.Length; i++)
            {
                var virtualKey = (ushort)VkKeyScanEx(charsToTest[i], keyboardLayout);
                if (virtualKey == 0)
                    return false;
                virtualKey = ExtractModifierAndWinScanCode(virtualKey, out modifier[i], out winScanCode[i]);
            }

            for (int i = 0; i < charsToTest.Length; i++)
                if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode[i], out var val))
                {
                    SendKey((KeyModifier)modifier[i], val);
                }
            return true;
        }

        private static ushort ExtractModifierAndWinScanCode(ushort virtualKey, out byte modifier, out ushort winScanCode)
        {
            modifier = (byte)KeyModifier.None;
            if (virtualKey >= shift)
            {
                if ((virtualKey & shift) > 0)
                {
                    virtualKey -= shift;
                    modifier |= (byte)KeyModifier.left_shift;
                }
                if ((virtualKey & ctrl) > 0)
                {
                    virtualKey -= ctrl;
                    modifier |= (byte)KeyModifier.left_control;
                }
                if ((virtualKey & alt) > 0)
                {
                    virtualKey -= alt;
                    modifier |= (byte)KeyModifier.left_alt;
                }
            }
            winScanCode = (ushort)PInvoke.User32.MapVirtualKey(virtualKey, PInvoke.User32.MapVirtualKeyTranslation.MAPVK_VK_TO_VSC);
            return virtualKey;
        }
        private static ushort ExtractModifierAndWinScanCode(ushort virtualKey, out KeyModifier modifier, out ushort winScanCode)
        {
            virtualKey = ExtractModifierAndWinScanCode(virtualKey, out byte keyMod, out winScanCode);
            modifier = (KeyModifier)keyMod;
            return virtualKey;
        }

        [DebuggerStepThrough]
        private void SendKey(KeyModifier modifier, int val)
        {
            buf[2] = (byte)(val >> 16);
            buf[3] = (byte)modifier;
            buf[5] = (byte)(val & 0xFF);
            vKeyboardStream.Write(buf);
            buf[3] = (byte)KeyModifier.None;
            buf[5] = 0x0;
            vKeyboardStream.Write(buf);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    vKeyboardStream.Dispose();
                }

                disposedValue = true;
            }
        }



        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
