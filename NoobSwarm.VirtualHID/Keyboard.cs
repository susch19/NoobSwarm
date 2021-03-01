using NoobSwarm.Hid;
using NoobSwarm.Hid.Reports;
using NoobSwarm.Makros;

using PInvoke;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NoobSwarm.VirtualHID
{
    public class Keyboard : IKeyboard, IDisposable
    {
        public bool CanUseDriver => canUseDriver;
        public bool PreferWinApi { get; set; }

        private bool disposedValue;
        private IntPtr keyboardLayout;
        private HidStream vKeyboardStream;
        private readonly byte[] buf;
        private readonly User32.INPUT[] inputs = new PInvoke.User32.INPUT[64];
        private const int shift = 0b100000000;
        private const int ctrl = 0b1000000000;
        private const int alt = 0b10000000000;


        private bool canUseDriver = false;
        private bool useWinAPI => PreferWinApi || !canUseDriver;


        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pwszBuff, int cchBuff, uint wFlags,
            IntPtr dwhkl);

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
            return;
            try
            {
                var devices = DeviceList.Local.GetHidDevices(0xAFFE).Where(x => x.ProductID == 0xCAFE).ToList();
                var controlDevice = devices.Where(x => x.GetReportDescriptor().Reports.Any(y => y.ReportID == 0x40))
                    .ToList();
                var kbDevice = devices.First(x => x.DevicePath.EndsWith("kbd"));

                if (controlDevice != default && controlDevice[0].TryOpen(out vKeyboardStream))
                {
                    var rd = controlDevice[0].GetReportDescriptor();
                    var rdkbd = kbDevice.GetReportDescriptor();
                    var inputRep = rd.Reports.First(x => x.ReportType == ReportType.Input);
                    var inputRepKbd = rdkbd.Reports.First(x => x.ReportType == ReportType.Input);
                    buf = inputRep.CreateBuffer();
                    buf[0] = 0x40;
                    buf[1] = (byte)inputRepKbd.Length; //length of subReport
                }
                canUseDriver = true;
            }
            catch (Exception ex)
            {
            }
        }

        private Makros.Key[] winKeys = new[] { Makros.Key.RWIN, Makros.Key.LWIN };
        private Makros.Key[] ctrlKeys = new[] { Makros.Key.LCONTROL, Makros.Key.RCONTROL };
        private Makros.Key[] shiftKeys = new[] { Makros.Key.LSHIFT, Makros.Key.RSHIFT };
        private Makros.Key[] menuKeys = new[] { Makros.Key.LMENU, Makros.Key.RMENU };
        public Task PlayMacro(IReadOnlyList<MakroManager.RecordKey> recKeysMem)
        {
            return Task.Run(async () =>
            {
                KeyModifier modifier = KeyModifier.None;
                for (int i = 0; i < recKeysMem.Count; i++)
                {
                    MakroManager.RecordKey rec = recKeysMem[i];
                    if (rec.TimeBeforePress > 0)
                        await Task.Delay(rec.TimeBeforePress);
                    if (
                        TestForModifierKey(rec, recKeysMem, shiftKeys, KeyModifier.Left_Shift, ref modifier, ref i)
                    || TestForModifierKey(rec, recKeysMem, menuKeys, KeyModifier.Left_Alt, ref modifier, ref i)
                    || TestForModifierKey(rec, recKeysMem, ctrlKeys, KeyModifier.Left_Control, ref modifier, ref i)
                    || TestForModifierKey(rec, recKeysMem, winKeys, KeyModifier.Left_Gui, ref modifier, ref i))
                    { }
                    else if (rec.Pressed)
                    {
                        SendVirtualKey((ushort)rec.Key, modifier);
                    }
                }

                SendVirtualKey(0, KeyModifier.None);
            });
        }


        public void SendVirtualKey(ushort key)
        {
            key = ExtractModifierAndWinScanCode(key, out KeyModifier modifier, out var winScanCode);

            if (winScanCode == 91)
            {
                modifier |= KeyModifier.Left_Gui;
                winScanCode -= 91;
            }

            if (useWinAPI)
            {
                //if (modifier > 0 && key == 0)
                //    SendKeyWinApi(KeyModifier.None, (ushort)User32.ScanCode.LWIN);
                //else
                SendKeyWinApi(modifier, winScanCode);
            }
            else if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
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
            if (useWinAPI)
            {
                //if (modifier > 0 && key == 0)
                //    SendKeyWinApi(KeyModifier.None, (ushort)User32.ScanCode.LWIN);
                //else
                SendKeyWinApi(modifier, winScanCode);
            }
            else if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
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
                if (useWinAPI)
                {
                    SendKeyWinApi(addModifier, winScanCode);
                }
                else if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
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
            if (useWinAPI)
            {
                SendKeyWinApi(modifier, winScanCode);
            }
            else if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
            {
                SendKey(modifier, val);
            }
        }

        public void SendChar(char charToTest, KeyModifier modifier)
        {
            var virtualKey = (ushort)VkKeyScanEx(charToTest, keyboardLayout);

            virtualKey = ExtractModifierAndWinScanCode(virtualKey, out KeyModifier addmodifier, out var winScanCode);
            modifier |= addmodifier;

            if (useWinAPI)
            {
                SendKeyWinApi(addmodifier, winScanCode);
            }
            else if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode, out var val))
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
                if (useWinAPI)
                {
                    SendKeysWinApi(modifier, winScanCode);
                    break;
                }
                else if (ScanCodeMapping.WinToUSB.TryGetValue(winScanCode[i], out var val))
                {
                    SendKey((KeyModifier)modifier[i], val);
                }

            return true;
        }

        private static ushort ExtractModifierAndWinScanCode(ushort virtualKey, out byte modifier,
            out ushort winScanCode)
        {
            modifier = (byte)KeyModifier.None;
            if (virtualKey >= shift)
            {
                if ((virtualKey & shift) > 0)
                {
                    virtualKey -= shift;
                    modifier |= (byte)KeyModifier.Left_Shift;
                }

                if ((virtualKey & ctrl) > 0)
                {
                    virtualKey -= ctrl;
                    modifier |= (byte)KeyModifier.Left_Control;
                }

                if ((virtualKey & alt) > 0)
                {
                    virtualKey -= alt;
                    modifier |= (byte)KeyModifier.Left_Alt;
                }
            }

            winScanCode =
                (ushort)PInvoke.User32.MapVirtualKey(virtualKey,
                    PInvoke.User32.MapVirtualKeyTranslation.MAPVK_VK_TO_VSC);
            return virtualKey;
        }

        private static ushort ExtractModifierAndWinScanCode(ushort virtualKey, out KeyModifier modifier,
            out ushort winScanCode)
        {
            virtualKey = ExtractModifierAndWinScanCode(virtualKey, out byte keyMod, out winScanCode);
            modifier = (KeyModifier)keyMod;
            return virtualKey;
        }

        private bool TestForModifierKey(MakroManager.RecordKey rec, IReadOnlyList<MakroManager.RecordKey> recKeys,
            ReadOnlySpan<Key> keysToCheck, KeyModifier modifierToAdd, ref KeyModifier modifier, ref int i)
        {
            var contains = false;

            bool Contains(MakroManager.RecordKey innerRecKey, ReadOnlySpan<Key> innerKeysToCheck)
            {
                foreach (var t in innerKeysToCheck)
                {
                    if (innerRecKey.Key == t)
                        return true;
                }

                return false;
            }


            if (!Contains(recKeys[i], keysToCheck))
                return false;

            if (recKeys.Count > i && recKeys[i].Pressed && !recKeys[i + 1].Pressed && Contains(recKeys[i + 1], keysToCheck))
            {
                SendVirtualKey(0, modifierToAdd);
                i++;
            }
            else
            {
                if (rec.Pressed)
                    modifier |= modifierToAdd;
                else if ((modifier & modifierToAdd) > 0)
                    modifier -= modifierToAdd;
            }

            return true;

        }

        [DebuggerStepThrough]
        private void SendKeyWinApi(KeyModifier addmodifier, ushort winScanCode)
        {
            int i = 0;
            var input = new PInvoke.User32.INPUT();

            // if ((addmodifier & (KeyModifier.Left_Alt | KeyModifier.Right_Alt)) > 0)
            //     AddInput(ref i, ref input, User32.ScanCode.LMENU, User32.KEYEVENTF.KEYEVENTF_SCANCODE);
            // else
            //     AddInput(ref i, ref input, User32.ScanCode.LMENU,
            //         User32.KEYEVENTF.KEYEVENTF_SCANCODE | User32.KEYEVENTF.KEYEVENTF_KEYUP);
            //
            // if ((addmodifier & (KeyModifier.Left_Control | KeyModifier.Right_Control)) > 0)
            //     AddInput(ref i, ref input, User32.ScanCode.CONTROL, User32.KEYEVENTF.KEYEVENTF_SCANCODE);
            // else
            //     AddInput(ref i, ref input, User32.ScanCode.CONTROL,
            //         User32.KEYEVENTF.KEYEVENTF_SCANCODE | User32.KEYEVENTF.KEYEVENTF_KEYUP);
            //
            // if (winScanCode != (ushort) User32.ScanCode.LWIN)
            //     if ((addmodifier & (KeyModifier.Left_Gui | KeyModifier.Right_Gui)) > 0)
            //         AddInput(ref i, ref input, User32.ScanCode.LWIN, User32.KEYEVENTF.KEYEVENTF_SCANCODE);
            //     else
            //         AddInput(ref i, ref input, User32.ScanCode.LWIN,
            //             User32.KEYEVENTF.KEYEVENTF_SCANCODE | User32.KEYEVENTF.KEYEVENTF_KEYUP);
            //
            // if ((addmodifier & (KeyModifier.Left_Shift | KeyModifier.Right_Shift)) > 0)
            //     AddInput(ref i, ref input, User32.ScanCode.SHIFT, User32.KEYEVENTF.KEYEVENTF_SCANCODE);
            // else
            //     AddInput(ref i, ref input, User32.ScanCode.SHIFT,
            //         User32.KEYEVENTF.KEYEVENTF_SCANCODE | User32.KEYEVENTF.KEYEVENTF_KEYUP);
            //
            if ((addmodifier & (KeyModifier.Left_Alt | KeyModifier.Right_Alt)) > 0)
                AddInput(ref i, ref input, User32.VirtualKey.VK_LMENU, 0);

            if ((addmodifier & (KeyModifier.Left_Control | KeyModifier.Right_Control)) > 0)
                AddInput(ref i, ref input, User32.VirtualKey.VK_LCONTROL, 0);

            if ((addmodifier & (KeyModifier.Left_Gui | KeyModifier.Right_Gui)) > 0)
                AddInput(ref i, ref input, User32.VirtualKey.VK_LWIN, 0);

            if ((addmodifier & (KeyModifier.Left_Shift | KeyModifier.Right_Shift)) > 0)
                AddInput(ref i, ref input, User32.VirtualKey.VK_SHIFT, 0);

            if (winScanCode > 0)
            {
                //AddInput(ref i, ref input, User32.VirtualKey.VK_LWIN, 0);
                //AddInput(ref i, ref input, User32.VirtualKey.VK_LWIN, User32.KEYEVENTF.KEYEVENTF_KEYUP);
                //}
                //else
                //{
                AddInput(ref i, ref input, (User32.ScanCode)winScanCode, User32.KEYEVENTF.KEYEVENTF_SCANCODE);
                AddInput(ref i, ref input, (User32.ScanCode)winScanCode,
                    User32.KEYEVENTF.KEYEVENTF_SCANCODE | User32.KEYEVENTF.KEYEVENTF_KEYUP);
            }

            if ((addmodifier & (KeyModifier.Left_Alt | KeyModifier.Right_Alt)) > 0)
                AddInput(ref i, ref input, User32.VirtualKey.VK_LMENU, User32.KEYEVENTF.KEYEVENTF_KEYUP);
            if ((addmodifier & (KeyModifier.Left_Control | KeyModifier.Right_Control)) > 0)
                AddInput(ref i, ref input, User32.VirtualKey.VK_LCONTROL, User32.KEYEVENTF.KEYEVENTF_KEYUP);

            if ((addmodifier & (KeyModifier.Left_Gui | KeyModifier.Right_Gui)) > 0)
                AddInput(ref i, ref input, User32.VirtualKey.VK_LWIN, User32.KEYEVENTF.KEYEVENTF_KEYUP);

            if ((addmodifier & (KeyModifier.Left_Shift | KeyModifier.Right_Shift)) > 0)
                AddInput(ref i, ref input, User32.VirtualKey.VK_SHIFT, User32.KEYEVENTF.KEYEVENTF_KEYUP);

            unsafe
            {
                User32.SendInput(i, inputs, sizeof(User32.INPUT));
            }

            for (int j = 0; j < i; j++)
            {
                inputs[j] = default;
            }
        }

        [DebuggerStepThrough]
        private void SendKeysWinApi(Span<byte> addmodifiers, Span<ushort> winScanCodes)
        {
            int i = 0;
            var input = new PInvoke.User32.INPUT();
            var localInputs = ArrayPool<User32.INPUT>.Shared.Rent(winScanCodes.Length * 10); //*10 ist worst case, if all modifiers are pressed and therefore release (4*2) plus the winScanCode 1*2 => 10x

            for (int o = 0; o < winScanCodes.Length; o++)
            {
                var addmodifier = (KeyModifier)addmodifiers[o];
                var winScanCode = winScanCodes[o];


                if ((addmodifier & (KeyModifier.Left_Alt | KeyModifier.Right_Alt)) > 0)
                    AddInput(ref i, ref input, User32.VirtualKey.VK_LMENU, 0, localInputs);

                if ((addmodifier & (KeyModifier.Left_Control | KeyModifier.Right_Control)) > 0)
                    AddInput(ref i, ref input, User32.VirtualKey.VK_LCONTROL, 0, localInputs);

                if ((addmodifier & (KeyModifier.Left_Gui | KeyModifier.Right_Gui)) > 0)
                    AddInput(ref i, ref input, User32.VirtualKey.VK_LWIN, 0, localInputs);

                if ((addmodifier & (KeyModifier.Left_Shift | KeyModifier.Right_Shift)) > 0)
                    AddInput(ref i, ref input, User32.VirtualKey.VK_SHIFT, 0, localInputs);

                if (winScanCode > 0)
                {

                    AddInput(ref i, ref input, (User32.ScanCode)winScanCode, User32.KEYEVENTF.KEYEVENTF_SCANCODE, localInputs);
                    AddInput(ref i, ref input, (User32.ScanCode)winScanCode,
                        User32.KEYEVENTF.KEYEVENTF_SCANCODE | User32.KEYEVENTF.KEYEVENTF_KEYUP, localInputs);
                }

                if ((addmodifier & (KeyModifier.Left_Alt | KeyModifier.Right_Alt)) > 0)
                    AddInput(ref i, ref input, User32.VirtualKey.VK_LMENU, User32.KEYEVENTF.KEYEVENTF_KEYUP, localInputs);
                if ((addmodifier & (KeyModifier.Left_Control | KeyModifier.Right_Control)) > 0)
                    AddInput(ref i, ref input, User32.VirtualKey.VK_LCONTROL, User32.KEYEVENTF.KEYEVENTF_KEYUP, localInputs);

                if ((addmodifier & (KeyModifier.Left_Gui | KeyModifier.Right_Gui)) > 0)
                    AddInput(ref i, ref input, User32.VirtualKey.VK_LWIN, User32.KEYEVENTF.KEYEVENTF_KEYUP, localInputs);

                if ((addmodifier & (KeyModifier.Left_Shift | KeyModifier.Right_Shift)) > 0)
                    AddInput(ref i, ref input, User32.VirtualKey.VK_SHIFT, User32.KEYEVENTF.KEYEVENTF_KEYUP, localInputs);

            }
            unsafe
            {
                User32.SendInput(i, localInputs, sizeof(User32.INPUT));
            }
            ArrayPool<User32.INPUT>.Shared.Return(localInputs);

        }

        private void AddInput(ref int i, ref User32.INPUT input, User32.ScanCode code, User32.KEYEVENTF flags)
            => AddInput(ref i, ref input, code, flags, inputs);

        private void AddInput(ref int i, ref User32.INPUT input, User32.ScanCode code, User32.KEYEVENTF flags, User32.INPUT[] inputs)
        {
            input.type = PInvoke.User32.InputType.INPUT_KEYBOARD; // 1 = Keyboard Input
            input.Inputs.ki.wScan = code;
            input.Inputs.ki.dwFlags = flags;
            inputs[i] = input;
            input = new PInvoke.User32.INPUT();
            i++;
        }


        private void AddInput(ref int i, ref User32.INPUT input, User32.VirtualKey code, User32.KEYEVENTF flags)
            => AddInput(ref i, ref input, code, flags, inputs);

        private void AddInput(ref int i, ref User32.INPUT input, User32.VirtualKey code, User32.KEYEVENTF flags, User32.INPUT[] inputs)
        {
            input.type = PInvoke.User32.InputType.INPUT_KEYBOARD; // 1 = Keyboard Input
            input.Inputs.ki.wVk = code;
            input.Inputs.ki.dwFlags = flags;
            inputs[i] = input;
            input = new PInvoke.User32.INPUT();
            i++;
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