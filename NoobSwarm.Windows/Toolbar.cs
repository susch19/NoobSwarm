using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NoobSwarm.Windows
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

    public record ToolbarButtonInfo(string ToolTip, int ProcessId, Icon Icon);

    // Most code from https://github.com/BarRaider/streamdeck-battery/blob/c1dd1d3c4d19bad3d36e7d53d9d28f4911831dc7/streamdeck-battery/Internal/ToolbarScanner.cs
    // and https://stackoverflow.com/questions/17724168/win32-how-to-access-button-on-toolbar
    public static class Toolbar
    {
        /// <summary>
        /// Gets the informationsof the toolbar buttons in the taskbar
        /// </summary>
        /// <param name="location">The location of the toolbar buttons</param>
        /// <returns>The informations for the found toolbar buttons</returns>
        public static IEnumerable<ToolbarButtonInfo> GetToolbarButtonInfos(ToolbarButtonLocation location)
        {
            return location switch
            {
                ToolbarButtonLocation.All => GetToolbarButtonInfos(GetSystemTrayHandle(true))
                                       .Concat(GetToolbarButtonInfos(GetSystemTrayHandle(false))),
                ToolbarButtonLocation.Visible => GetToolbarButtonInfos(GetSystemTrayHandle(true)),
                ToolbarButtonLocation.Hidden => GetToolbarButtonInfos(GetSystemTrayHandle(false)),
                _ => Enumerable.Empty<ToolbarButtonInfo>(),
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

        private static IEnumerable<ToolbarButtonInfo> GetToolbarButtonInfos(IntPtr systemTrayHandle)
        {
            if (systemTrayHandle == IntPtr.Zero)
            {
                ThrowNative();
                yield break;
            }

            var count = SendMessage(systemTrayHandle, TB_BUTTONCOUNT, IntPtr.Zero, IntPtr.Zero);
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
                using var tbButtonInfoWStruct = new ProcessStruct<TBBUTTONINFOW>(hProcess);
                using var tbButtonStruct = new ProcessStruct<TBBUTTON>(hProcess);
                for (var i = 0; i < count; i++)
                {
                    var btn = new TBBUTTONINFOW
                    {
                        cbSize = Unsafe.SizeOf<TBBUTTONINFOW>(),
                        dwMask = TBIF_BYINDEX | TBIF_COMMAND
                    };

                    var toolTip = "";
                    var iconPid = 0;
                    Icon icon = null;

                    if (tbButtonInfoWStruct.TrySetValue(btn))
                    {
                        // we want the identifier
                        var res = SendMessage(systemTrayHandle, TB_GETBUTTONINFOW, (IntPtr)i, tbButtonInfoWStruct.GetBuffer());
                        if (res >= 0 && tbButtonInfoWStruct.TryGetValue(out btn))
                        {
                            var tbButton = new TBBUTTON
                            {
                                idCommand = btn.idCommand
                            };

                            if (tbButtonStruct.TrySetValue(tbButton))
                            {
                                res = SendMessage(systemTrayHandle, TB_GETBUTTON, (IntPtr)i, tbButtonStruct.GetBuffer());
                                if (res >= 0 && tbButtonStruct.TryGetValue(out tbButton))
                                {
                                    using var trayDataStruct = new ProcessStruct<TrayData>(hProcess, tbButton.dwData);
                                    if (trayDataStruct.TryGetValue(out var trayData))
                                    {
                                        _ = GetWindowThreadProcessId(trayData.hWnd, out iconPid);
                                        for (int tries = 0; tries < 2; tries++)
                                        {
                                            try
                                            {
                                                if (GetIconInfo(trayData.hIcon, out var iconInfo))
                                                    icon = (Icon)Icon.FromHandle(trayData.hIcon).Clone();

                                                break;
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }

                            // Try get icon from process
                            if (icon is null && iconPid > 0)
                            {
                                var process = Process.GetProcessById(iconPid);

                                if (process is not null && !string.IsNullOrEmpty(process.MainModule?.FileName))
                                {
                                    try
                                    {
                                        icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }

                            // now get display text using the identifier
                            // first pass we ask for size
                            var textSize = SendMessage(systemTrayHandle, TB_GETBUTTONTEXTW, (IntPtr)btn.idCommand, IntPtr.Zero);
                            if (textSize != -1)
                            {
                                // we need to allocate for the terminating zero and unicode
                                var utextSize = (IntPtr)((1 + textSize) * 2);
                                using var textBuffer = new ProcessBuffer(hProcess, utextSize);
                                if (SendMessage(systemTrayHandle, TB_GETBUTTONTEXTW, (IntPtr)btn.idCommand, textBuffer.GetBuffer()) == textSize)
                                {
                                    var localBuffer = Marshal.AllocHGlobal(utextSize.ToInt32());

                                    try
                                    {
                                        if (textBuffer.TryReadStruct(utextSize, localBuffer))
                                            toolTip = Marshal.PtrToStringUni(localBuffer);
                                    }
                                    finally
                                    {
                                        Marshal.FreeHGlobal(localBuffer);
                                    }
                                }
                            }

                            yield return new ToolbarButtonInfo(toolTip, iconPid, icon);
                        }
                    }
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
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32", SetLastError = true)]
        private unsafe static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("user32", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, int flAllocationType, int flProtect);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, int dwFreeType);

        [DllImport("user32")]
        private static extern nint SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [StructLayout(LayoutKind.Sequential)]
        private struct ICONINFO
        {
            /// <summary>
            /// Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies
            /// an icon; FALSE specifies a cursor.
            /// </summary>
            public bool fIcon;

            /// <summary>
            /// Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot
            /// spot is always in the center of the icon, and this member is ignored.
            /// </summary>
            public Int32 xHotspot;

            /// <summary>
            /// Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot
            /// spot is always in the center of the icon, and this member is ignored.
            /// </summary>
            public Int32 yHotspot;

            /// <summary>
            /// (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon,
            /// this bitmask is formatted so that the upper half is the icon AND bitmask and the lower half is
            /// the icon XOR bitmask. Under this condition, the height should be an even multiple of two. If
            /// this structure defines a color icon, this mask only defines the AND bitmask of the icon.
            /// </summary>
            public IntPtr hbmMask;

            /// <summary>
            /// (HBITMAP) Handle to the icon color bitmap. This member can be optional if this
            /// structure defines a black and white icon. The AND bitmask of hbmMask is applied with the SRCAND
            /// flag to the destination; subsequently, the color bitmap is applied (using XOR) to the
            /// destination by using the SRCINVERT flag.
            /// </summary>
            public IntPtr hbmColor;
        }

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

        [StructLayout(LayoutKind.Sequential)]
        private struct TrayData
        {
            public IntPtr hWnd;
            public uint uID;
            public uint uCallbackMessage;
            private uint reserved0;
            private uint reserved1;
            public IntPtr hIcon;
        }

        #region Code from https://github.com/dahall/Vanara/blob/30930565e685d0c1e991825b4ebecae3effe847a/PInvoke/ComCtl32/CommCtrl.Toolbar.cs

        /// <summary>Contains information about a button in a toolbar.</summary>
        // typedef struct { int iBitmap; int idCommand; BYTE fsState; BYTE fsStyle;#ifdef _WIN64 BYTE bReserved[6];#else #if defined(_WIN32)
        // BYTE bReserved[2];#endif #endif DWORD_PTR dwData; INT_PTR iString;} TBBUTTON, *PTBBUTTON, *LPTBBUTTON; https://msdn.microsoft.com/en-us/library/windows/desktop/bb760476(v=vs.85).aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct TBBUTTON
        {
            /// <summary>
            /// Zero-based index of the button image. Set this member to I_IMAGECALLBACK, and the toolbar will send the TBN_GETDISPINFO
            /// notification code to retrieve the image index when it is needed.
            /// <para>
            /// Version 5.81. Set this member to I_IMAGENONE to indicate that the button does not have an image.The button layout will not
            /// include any space for a bitmap, only text.
            /// </para>
            /// <para>
            /// If the button is a separator, that is, if fsStyle is set to BTNS_SEP, iBitmap determines the width of the separator, in
            /// pixels.For information on selecting button images from image lists, see TB_SETIMAGELIST message.
            /// </para>
            /// </summary>
            public int iBitmap;

            /// <summary>
            /// Command identifier associated with the button. This identifier is used in a WM_COMMAND message when the button is chosen.
            /// </summary>
            public int idCommand;

            // Funky holder to make preprocessor directives work
            private TBBUTTON_U union;

            /// <summary>Button state flags.</summary>
            public TBSTATE fsState { get => union.fsState; set => union.fsState = value; }

            /// <summary>Button style.</summary>
            public ToolbarStyle fsStyle { get => union.fsStyle; set => union.fsStyle = value; }

            /// <summary>Application-defined value.</summary>
            public IntPtr dwData;

            /// <summary>Zero-based index of the button string, or a pointer to a string buffer that contains text for the button.</summary>
            public IntPtr iString;

            [StructLayout(LayoutKind.Explicit, Pack = 1)]
            private struct TBBUTTON_U
            {
                [FieldOffset(0)] private readonly IntPtr bReserved;
                [FieldOffset(0)] public TBSTATE fsState;
                [FieldOffset(1)] public ToolbarStyle fsStyle;
            }
        }

        /// <summary>Toolbar Control and Button Styles</summary>
        [Flags]
        public enum ToolbarStyle : ushort
        {
            /// <summary>
            /// Allows users to change a toolbar button's position by dragging it while holding down the ALT key. If this style is not
            /// specified, the user must hold down the SHIFT key while dragging a button. Note that the CCS_ADJUSTABLE style must be
            /// specified to enable toolbar buttons to be dragged.
            /// </summary>
            TBSTYLE_ALTDRAG = 0x0400,

            /// <summary>Equivalent to BTNS_AUTOSIZE. Use TBSTYLE_AUTOSIZE for version 4.72 and earlier.</summary>
            TBSTYLE_AUTOSIZE = 0x0010,

            /// <summary>Equivalent to BTNS_BUTTON. Use TBSTYLE_BUTTON for version 4.72 and earlier.</summary>
            TBSTYLE_BUTTON = 0x0000,

            /// <summary>Equivalent to BTNS_CHECK. Use TBSTYLE_CHECK for version 4.72 and earlier.</summary>
            TBSTYLE_CHECK = 0x0002,

            /// <summary>Equivalent to BTNS_CHECKGROUP. Use TBSTYLE_CHECKGROUP for version 4.72 and earlier.</summary>
            TBSTYLE_CHECKGROUP = (TBSTYLE_GROUP | TBSTYLE_CHECK),

            /// <summary>Version 4.70. Generates NM_CUSTOMDRAW notification codes when the toolbar processes WM_ERASEBKGND messages.</summary>
            TBSTYLE_CUSTOMERASE = 0x2000,

            /// <summary>Equivalent to BTNS_DROPDOWN. Use TBSTYLE_DROPDOWN for version 4.72 and earlier.</summary>
            TBSTYLE_DROPDOWN = 0x0008,

            /// <summary>
            /// Version 4.70. Creates a flat toolbar. In a flat toolbar, both the toolbar and the buttons are transparent and hot-tracking is
            /// enabled. Button text appears under button bitmaps. To prevent repainting problems, this style should be set before the
            /// toolbar control becomes visible.
            /// </summary>
            TBSTYLE_FLAT = 0x0800,

            /// <summary>Equivalent to BTNS_GROUP. Use TBSTYLE_GROUP for version 4.72 and earlier.</summary>
            TBSTYLE_GROUP = 0x0004,

            /// <summary>
            /// Version 4.70. Creates a flat toolbar with button text to the right of the bitmap. Otherwise, this style is identical to
            /// TBSTYLE_FLAT. To prevent repainting problems, this style should be set before the toolbar control becomes visible.
            /// </summary>
            TBSTYLE_LIST = 0x1000,

            /// <summary>Equivalent to BTNS_NOPREFIX. Use TBSTYLE_NOPREFIX for version 4.72 and earlier.</summary>
            TBSTYLE_NOPREFIX = 0x0020,

            /// <summary>
            /// Version 4.71. Generates TBN_GETOBJECT notification codes to request drop target objects when the cursor passes over toolbar buttons.
            /// </summary>
            TBSTYLE_REGISTERDROP = 0x4000,

            /// <summary>Equivalent to BTNS_SEP. Use TBSTYLE_SEP for version 4.72 and earlier.</summary>
            TBSTYLE_SEP = 0x0001,

            /// <summary>Creates a tooltip control that an application can use to display descriptive text for the buttons in the toolbar.</summary>
            TBSTYLE_TOOLTIPS = 0x0100,

            /// <summary>
            /// Version 4.71. Creates a transparent toolbar. In a transparent toolbar, the toolbar is transparent but the buttons are not.
            /// Button text appears under button bitmaps. To prevent repainting problems, this style should be set before the toolbar control
            /// becomes visible.
            /// </summary>
            TBSTYLE_TRANSPARENT = 0x8000,

            /// <summary>
            /// Creates a toolbar that can have multiple lines of buttons. Toolbar buttons can "wrap" to the next line when the toolbar
            /// becomes too narrow to include all buttons on the same line. When the toolbar is wrapped, the break will occur on either the
            /// rightmost separator or the rightmost button if there are no separators on the bar. This style must be set to display a
            /// vertical toolbar control when the toolbar is part of a vertical rebar control. This style cannot be combined with CCS_VERT.
            /// </summary>
            TBSTYLE_WRAPABLE = 0x0200,
        }

        /// <summary>State values used by TB_GETSTATE and TB_SETSTATE.</summary>
        [Flags]
        public enum TBSTATE : byte
        {
            /// <summary>The button has the TBSTYLE_CHECK style and is being clicked.</summary>
            TBSTATE_CHECKED = 0x01,

            /// <summary>Version 4.70. The button's text is cut off and an ellipsis is displayed.</summary>
            TBSTATE_ELLIPSES = 0x40,

            /// <summary>The button accepts user input. A button that does not have this state is grayed.</summary>
            TBSTATE_ENABLED = 0x04,

            /// <summary>The button is not visible and cannot receive user input.</summary>
            TBSTATE_HIDDEN = 0x08,

            /// <summary>The button is grayed.</summary>
            TBSTATE_INDETERMINATE = 0x10,

            /// <summary>Version 4.71. The button is marked. The interpretation of a marked item is dependent upon the application.</summary>
            TBSTATE_MARKED = 0x80,

            /// <summary>The button is being clicked.</summary>
            TBSTATE_PRESSED = 0x02,

            /// <summary>The button is followed by a line break. The button must also have the TBSTATE_ENABLED state.</summary>
            TBSTATE_WRAP = 0x20,
        }

        #endregion

        private class ProcessStruct<T> : IDisposable where T : unmanaged
        {
            private readonly ProcessBuffer buffer;

            public unsafe ProcessStruct(IntPtr processHandle)
            {
                buffer = new ProcessBuffer(processHandle, (IntPtr)sizeof(T));
            }
            public unsafe ProcessStruct(IntPtr processHandle, IntPtr buffer)
            {
                this.buffer = new ProcessBuffer(processHandle, buffer, (IntPtr)sizeof(T));
            }

            public IntPtr GetBuffer()
            {
                return buffer.GetBuffer();
            }

            public void SetValue(T value)
            {
                buffer.WriteStruct<T>(value);
            }

            public bool TrySetValue(T value)
            {
                return buffer.TryWriteStruct<T>(value);
            }

            public T GetValue()
            {
                return buffer.ReadStruct<T>();
            }

            public bool TryGetValue(out T value)
            {
                return buffer.TryReadStruct<T>(out value);
            }

            public void Dispose()
            {
                buffer.Dispose();
            }
        }

        private class ProcessBuffer : IDisposable
        {
            private static void ThrowNative()
            {
                var error = Marshal.GetLastWin32Error();

                if (error != 0)
                    throw new Win32Exception(error);
            }
            private readonly IntPtr processHandle;
            private readonly bool owner;

            private readonly IntPtr buffer;
            private readonly IntPtr bufferSize;

            public ProcessBuffer(IntPtr processHandle, IntPtr buffer, IntPtr structSize)
            {
                owner = false;
                this.processHandle = processHandle;
                this.buffer = buffer;
                bufferSize = structSize;
            }

            public ProcessBuffer(IntPtr processHandle, IntPtr structSize)
            {
                owner = true;
                this.processHandle = processHandle;
                bufferSize = structSize;
                buffer = VirtualAllocEx(processHandle, IntPtr.Zero, structSize, MEM_COMMIT, PAGE_READWRITE);
                if (buffer == IntPtr.Zero)
                {
                    ThrowNative();
                    return;
                }
            }

            public IntPtr GetBuffer()
            {
                return buffer;
            }

            public unsafe T ReadStruct<T>(int offset = 0) where T : unmanaged
            {
                var size = (nint)sizeof(T);
                if (offset + size > bufferSize)
                    throw new Exception("Struct too big!");
                T ret = default;
                if (!ReadProcessMemory(processHandle, buffer + offset, &ret, size, out var read))
                    ThrowNative();
                if (read < size)
                    throw new Exception("Read too little data!");

                return ret;
            }

            public unsafe bool TryReadStruct<T>(out T value, int offset = 0) where T : unmanaged
            {
                value = default;

                return TryReadStruct(sizeof(T), out value, offset);
            }

            public unsafe bool TryReadStruct<T>(nint size, out T value, int offset = 0) where T : unmanaged
            {
                value = default;

                if (offset + size > bufferSize)
                    return false;

                T ret = default;
                if (!ReadProcessMemory(processHandle, buffer + offset, &ret, size, out var read))
                    return false;

                if (read < size)
                    return false;

                value = ret;
                return true;
            }

            public unsafe bool TryReadStruct(nint size, IntPtr value, int offset = 0)
            {
                if (offset + size > bufferSize)
                    return false;

                if (!ReadProcessMemory(processHandle, buffer + offset, value, size, out var read))
                    return false;

                if (read < size)
                    return false;

                return true;
            }

            public unsafe void WriteStruct<T>(T value, int offset = 0) where T : unmanaged
            {
                var size = (nint)sizeof(T);
                if (offset + size > bufferSize)
                    throw new Exception("Struct too big!");

                if (!WriteProcessMemory(processHandle, buffer + offset, new IntPtr(&value), size, out var written))
                    ThrowNative();
                if (written < size)
                    throw new Exception("Too little data written!");
            }

            public unsafe bool TryWriteStruct<T>(T value, int offset = 0) where T : unmanaged
            {
                var size = (nint)sizeof(T);
                if (offset + size > bufferSize)
                    return false;

                if (!WriteProcessMemory(processHandle, buffer + offset, new IntPtr(&value), size, out var written))
                    return false;

                return (written >= size);
            }

            public void Dispose()
            {
                if (owner)
                    VirtualFreeEx(processHandle, buffer, IntPtr.Zero, MEM_RELEASE);
            }
        }
    }
}
