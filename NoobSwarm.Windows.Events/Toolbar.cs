using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace NoobSwarm.Windows.Events
{
    // Most code from https://github.com/BarRaider/streamdeck-battery/blob/c1dd1d3c4d19bad3d36e7d53d9d28f4911831dc7/streamdeck-battery/Internal/ToolbarScanner.cs
    // and https://stackoverflow.com/questions/17724168/win32-how-to-access-button-on-toolbar
    public static class Toolbar
    {
        /// <summary>
        /// Location of the toolbar buttons
        /// </summary>
        public enum ToolbarButtonLocation
        {
            /// <summary>
            /// All toolbar buttons
            /// </summary>
            All,

            /// <summary>
            /// The visible toolbar buttons
            /// </summary>
            Visible,

            /// <summary>
            /// The hidden toolbar buttons
            /// </summary>
            Hidden,
        }

        /// <summary>
        /// Gets the tooltip text of the toolbar buttons in the taskbar
        /// </summary>
        /// <param name="location">The location of the toolbar buttons</param>
        /// <returns>The tooltip text for the found toolbar buttons</returns>
        public static IEnumerable<string> GetToolbarButtonsText(ToolbarButtonLocation location)
        {
            return location switch
            {
                ToolbarButtonLocation.All => GetToolbarButtonsText(GetSystemTrayHandle(true))
                                       .Concat(GetToolbarButtonsText(GetSystemTrayHandle(false))),
                ToolbarButtonLocation.Visible => GetToolbarButtonsText(GetSystemTrayHandle(true)),
                ToolbarButtonLocation.Hidden => GetToolbarButtonsText(GetSystemTrayHandle(false)),
                _ => Enumerable.Empty<string>(),
            };
        }

        private static IntPtr GetSystemTrayHandle(bool visible)
        {
            if (visible)
            {
                var hwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
                hwnd = FindWindowEx(hwnd, IntPtr.Zero, "TrayNotifyWnd", null);
                hwnd = FindWindowEx(hwnd, IntPtr.Zero, "SysPager", null);
                return FindWindowEx(hwnd, IntPtr.Zero, "ToolbarWindow32", null);
            }
            else
            {
                var hwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "NotifyIconOverflowWindow", null);
                return FindWindowEx(hwnd, IntPtr.Zero, "ToolbarWindow32", null);
            }
        }

        private static IEnumerable<string> GetToolbarButtonsText(IntPtr systemTrayHandle)
        {
            if (systemTrayHandle == IntPtr.Zero)
            {
                ThrowNative();
                yield break;
            }

            var count = SendMessage(systemTrayHandle, TB_BUTTONCOUNT, IntPtr.Zero, IntPtr.Zero).ToInt32();
            if (count == 0)
            {
                ThrowNative();
                yield break;
            }

            _ = GetWindowThreadProcessId(systemTrayHandle, out var pid);
            var hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            if (hProcess == IntPtr.Zero)
            {
                ThrowNative();
                yield break;
            }

            try
            {
                var size = (IntPtr)Marshal.SizeOf<TBBUTTONINFOW>();
                var buffer = VirtualAllocEx(hProcess, IntPtr.Zero, size, MEM_COMMIT, PAGE_READWRITE);

                if (buffer == IntPtr.Zero)
                {
                    ThrowNative();
                    yield break;
                }

                try
                {
                    for (var i = 0; i < count; i++)
                    {
                        var btn = new TBBUTTONINFOW
                        {
                            cbSize = size.ToInt32(),
                            dwMask = TBIF_BYINDEX | TBIF_COMMAND
                        };

                        if (WriteProcessMemory(hProcess, buffer, ref btn, size, out _))
                        {
                            // we want the identifier
                            if (SendMessage(systemTrayHandle, TB_GETBUTTONINFOW, (IntPtr)i, buffer).ToInt32() >= 0 &&
                                ReadProcessMemory(hProcess, buffer, ref btn, size, out _))
                            {
                                // now get display text using the identifier
                                // first pass we ask for size
                                var textSize = SendMessage(systemTrayHandle, TB_GETBUTTONTEXTW, (IntPtr)btn.idCommand, IntPtr.Zero);
                                if (textSize.ToInt32() != -1)
                                {
                                    // we need to allocate for the terminating zero and unicode
                                    var utextSize = (IntPtr)((1 + textSize.ToInt32()) * 2);
                                    var textBuffer = VirtualAllocEx(hProcess, IntPtr.Zero, utextSize, MEM_COMMIT, PAGE_READWRITE);
                                    if (textBuffer != IntPtr.Zero)
                                    {
                                        if (SendMessage(systemTrayHandle, TB_GETBUTTONTEXTW, (IntPtr)btn.idCommand, textBuffer) == textSize)
                                        {
                                            var localBuffer = Marshal.AllocHGlobal(utextSize.ToInt32());

                                            try
                                            {
                                                if (ReadProcessMemory(hProcess, textBuffer, localBuffer, utextSize, out _))
                                                    yield return Marshal.PtrToStringUni(localBuffer);
                                            }
                                            finally
                                            {
                                                Marshal.FreeHGlobal(localBuffer);
                                            }
                                        }

                                        VirtualFreeEx(hProcess, textBuffer, IntPtr.Zero, MEM_RELEASE);
                                    }
                                }
                            }
                        }
                    }

                }
                finally
                {
                    VirtualFreeEx(hProcess, buffer, IntPtr.Zero, MEM_RELEASE);
                }
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        private static void ThrowNative()
        {
            var error = Marshal.GetLastWin32Error();

            if (error != 0)
                throw new Win32Exception(error);
        }

        private static bool IsWindowsVistaOrAbove() => Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6;
        private static int PROCESS_ALL_ACCESS => IsWindowsVistaOrAbove() ? 0x001FFFFF : 0x001F0FFF;

        private const int TBIF_BYINDEX = unchecked((int)0x80000000); // this specifies that the wparam in Get/SetButtonInfo is an index, not id
        private const int TBIF_COMMAND = 0x00000020;

        // CommCtrl.h
        private const int WM_USER = 0x0400;
        private const int MEM_COMMIT = 0x1000;
        private const int MEM_RELEASE = 0x8000;
        private const int PAGE_READWRITE = 0x4;
        private const int TB_GETBUTTON = (WM_USER + 23);
        private const int TB_BUTTONCOUNT = WM_USER + 24;
        private const int TB_GETBUTTONINFOW = WM_USER + 63;
        private const int TB_GETBUTTONTEXTW = WM_USER + 75;

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref TBBUTTONINFOW lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref TBBUTTONINFOW lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("user32", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, int flAllocationType, int flProtect);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, int dwFreeType);

        [DllImport("user32")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);


        [StructLayout(LayoutKind.Sequential)]
        private struct TBBUTTONINFOW
        {
            public int cbSize;
            public int dwMask;
            public int idCommand;
            public int iImage;
            public byte fsState;
            public byte fsStyle;
            public short cx;
            public IntPtr lParam;
            public IntPtr pszText;
            public int cchText;
        }
    }
}
