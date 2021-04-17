using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NoobSwarm.Makros;

namespace NoobSwarm.GenericKeyboard
{
    public class LowLevelKeyboardHookWindows : KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void KeyboardTestCallback(int code, int virtualKey, int scanCode, int wParam);

        [DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void StartHook();

        [DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.StdCall)]

        private static extern void SetCallback(IntPtr aCallback);
        [DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void start_message_loop();

        [DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void StopHook();

        [DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetDispatchKeyPress(bool dispatch);

        [DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool AddKeyToSuppress(int key);
        [DllImport("libs\\KeyboardHooktestDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool RemoveKeyToSuppress(int key);

        private Task hookWithMessageLoop;
        private CancellationTokenSource source;


        private Key currentStartKey = (Key)(0xFFFF);
        public override void HookKeyboard(Key startKey)
        {
            if (hookWithMessageLoop is not null || source is not null)
                return;

            if (currentStartKey == (Key) 0xFFFF)
            {
                RemoveKeyToSuppress(currentStartKey);
            }
            AddKeyToSuppress(startKey);
            source = new CancellationTokenSource();
            hookWithMessageLoop = Task.Run(MessageLoopCpp, source.Token).ContinueWith((a) => UnHookKeyboard());
        }

        public bool AddKeyToSuppress(Key key) => AddKeyToSuppress((int)key);
        public bool RemoveKeyToSuppress(Key key) => RemoveKeyToSuppress((int)key);
        

        public void UnHookKeyboard()
        {
            if (source is not null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
            if (hookWithMessageLoop is not null)
                hookWithMessageLoop = null;
            StopHook();
            SetDispatchKeyPress(true);
        }

        public void SetSupressKeyPress(bool supress = true)
        {
            SetDispatchKeyPress(!supress);
        }

        private void MessageLoopCpp()
        {
            var del = new KeyboardTestCallback(HookCallback);
            StartHook();
            SetCallback(Marshal.GetFunctionPointerForDelegate(del));
            start_message_loop();
        }

        private void HookCallback(int nCode, int vkCode, int scanCode, int wParam)
        {
            if (nCode >= 0 && wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
            {
                RaiseOnKeyPressed((Key)vkCode);
            }
            else if (nCode >= 0 && wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
            {
                RaiseOnKeyUnpressed((Key)vkCode);
            }
        }

        public override void Dispose()
        {
            if (source is not null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
            hookWithMessageLoop = null;
            StopHook();
        }
    }
}
