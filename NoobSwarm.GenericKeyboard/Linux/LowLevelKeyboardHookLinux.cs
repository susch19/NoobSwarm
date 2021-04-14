using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NoobSwarm.Makros;

using Window = System.UInt64;
using Time = System.UInt64;
using Bool = System.Int32;
namespace NoobSwarm.GenericKeyboard.Linux
{
    public class LowLevelKeyboardHookLinux : KeyboardHook
    {
        struct Display{}
        private Task hookWithMessageLoop;
        private CancellationTokenSource source;
       

        private IntPtr dpy;
        private Window root;

        private int startKeyCode;

        private Key startVK;
        
        public override void HookKeyboard(Key startKey)
        {
            startVK = startKey;
            dpy     = X11Native.XOpenDisplay(IntPtr.Zero);
            root    = X11Native.DefaultRootWindow(dpy);

            var xk = X11KeyMapping.VKToXK(startKey);
            startKeyCode = X11Native.XKeysymToKeycode(dpy, (ulong)xk);
            
            uint[] ignoreModifiers = {X11Native.LockMask, X11Native.Mod2Mask, X11Native.Mod3Mask , X11Native.Mod5Mask };
     
            GrabKey(dpy, startKeyCode, 0, root, 0, X11Native.GrabModeAsync, X11Native.GrabModeAsync, ignoreModifiers);

            X11Native.XSelectInput(dpy, root, X11Native.KeyPressMask);
            
            source = new CancellationTokenSource();
            hookWithMessageLoop = Task.Run(() => MessageLoop(source.Token), source.Token).ContinueWith((a) => UnHookKeyboard());
        }

        private bool isInHotKey = false;
        private void HandleHotKeyStart(ref X11Native.XKeyEvent ev)
        {
            switch (ev.type)
            {
                case X11Native.KeyPress:
                    isInHotKey = true;
                    RaiseOnKeyPressed(startVK);
                    
                    X11Native.XGrabKeyboard(dpy, root, 1, X11Native.GrabModeAsync, X11Native.GrabModeAsync, X11Native.CurrentTime);
                    break;
            }
        }

        private void HandleInHotKey(ref X11Native.XKeyEvent ev)
        {
            var keySym = (XK)X11Native.XLookupKeysym(ref ev, 0);

            var vk = X11KeyMapping.XKToVK(keySym);
            switch (ev.type)
            {
                case X11Native.KeyPress:
                    RaiseOnKeyPressed(vk);
                    break;
                case X11Native.KeyRelease:
                    if (ev.keycode == startKeyCode)
                    {
                        X11Native.XUngrabKeyboard(dpy, X11Native.CurrentTime);
                        isInHotKey = false;
                        RaiseOnKeyUnpressed(startVK);
                    }
                    else
                    {
                        RaiseOnKeyUnpressed(vk);
                    }
                    break;
            }
        }

        private void MessageLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    X11Native.XKeyEvent keyEvent = default;
                    X11Native.XNextEvent(dpy, ref keyEvent);

                    if (keyEvent.type != X11Native.KeyPress && keyEvent.type != X11Native.KeyRelease)
                        continue;

                    if (keyEvent.keycode == this.startKeyCode && !isInHotKey)
                    {
                        HandleHotKeyStart(ref keyEvent);
                    }
                    else if (isInHotKey)
                    {
                        HandleInHotKey(ref keyEvent);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public void UnHookKeyboard()
        {
            source.Cancel();
            
            X11Native.XUngrabKey(dpy,startKeyCode,0 ,root);

            X11Native.XCloseDisplay(dpy);
        }
        
        void GrabKey(IntPtr dpy, int keycode, uint modifiers, Window grabWindow, Bool ownerEvents, int pointerMode, int keyboardMode, uint[] ignoreModifiers)
        {

            int count = 1 << ignoreModifiers.Length;
            for (int j=0;j<count;j++)
            {
                uint mod = 0;
                for (int i=0;i<ignoreModifiers.Length;i++)
                {
                    int set = (j / (1 << i)) & 1;
                    if (set != 0)
                        mod |= ignoreModifiers[i];
                }
                X11Native.XGrabKey(dpy, keycode, modifiers | mod, grabWindow, ownerEvents, pointerMode, keyboardMode);
            }

        }

        public override void Dispose()
        {
            source.Cancel();
            hookWithMessageLoop.Wait();
        }
    }
}