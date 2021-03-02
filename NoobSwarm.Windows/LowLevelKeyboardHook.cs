using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NoobSwarm.Windows
{
    public class LowLevelKeyboardHook : IDisposable
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


        public event EventHandler<Makros.Key> OnKeyPressed;
        public event EventHandler<Makros.Key> OnKeyUnpressed;

        private Task hookWithMessageLoop;
        private CancellationTokenSource source;


        public void HookKeyboard()
        {
            if (hookWithMessageLoop is not null || source is not null)
                return;

            source = new CancellationTokenSource();
            hookWithMessageLoop = Task.Run(MessageLoopCpp, source.Token).ContinueWith((a) => UnHookKeyboard());
        }

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
                OnKeyPressed?.Invoke(this, (Makros.Key)vkCode);
            }
            else if (nCode >= 0 && wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
            {
                OnKeyUnpressed?.Invoke(this, (Makros.Key)vkCode);
            }
        }

        public void Dispose()
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
