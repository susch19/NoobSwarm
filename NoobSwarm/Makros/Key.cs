using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.Makros
{
    public enum Key : ushort
    {
        //
        // Summary:
        //     This is an addendum to use on functions in which you have to pass a zero value
        //     to represent no key code
        NO_KEY = 0,
        //
        // Summary:
        //     Left mouse button
        LBUTTON = 1,
        //
        // Summary:
        //     Right mouse button
        RBUTTON = 2,
        //
        // Summary:
        //     Control-break processing
        CANCEL = 3,
        //
        // Summary:
        //     Middle mouse button (three-button mouse)
        //
        // Remarks:
        //     NOT contiguous with L and R buttons
        MBUTTON = 4,
        //
        // Summary:
        //     X1 mouse button
        //
        // Remarks:
        //     NOT contiguous with L and R buttons
        XBUTTON1 = 5,
        //
        // Summary:
        //     X2 mouse button
        //
        // Remarks:
        //     NOT contiguous with L and R buttons
        XBUTTON2 = 6,
        //
        // Summary:
        //     BACKSPACE key
        BACK = 8,
        //
        // Summary:
        //     TAB key
        TAB = 9,
        //
        // Summary:
        //     CLEAR key
        CLEAR = 12,
        //
        // Summary:
        //     RETURN key
        RETURN = 13,
        //
        // Summary:
        //     SHIFT key
        SHIFT = 16,
        //
        // Summary:
        //     CONTROL key
        CONTROL = 17,
        //
        // Summary:
        //     ALT key
        MENU = 18,
        //
        // Summary:
        //     PAUSE key
        PAUSE = 19,
        //
        // Summary:
        //     CAPS LOCK key
        CAPITAL = 20,
        //
        // Summary:
        //     IME Kana mode
        KANA = 21,
        //
        // Summary:
        //     IME Hanguel mode (maintained for compatibility; use PInvoke.User32.VirtualKey.HANGUL)
        HANGEUL = 21,
        //
        // Summary:
        //     IME Hangul mode
        HANGUL = 21,
        //
        // Summary:
        //     IME Junja mode
        JUNJA = 23,
        //
        // Summary:
        //     IME final mode
        FINAL = 24,
        //
        // Summary:
        //     IME Hanja mode
        HANJA = 25,
        //
        // Summary:
        //     IME Kanji mode
        KANJI = 25,
        //
        // Summary:
        //     ESC key
        ESCAPE = 27,
        //
        // Summary:
        //     IME convert
        CONVERT = 28,
        //
        // Summary:
        //     IME nonconvert
        NONCONVERT = 29,
        //
        // Summary:
        //     IME accept
        ACCEPT = 30,
        //
        // Summary:
        //     IME mode change request
        MODECHANGE = 31,
        //
        // Summary:
        //     SPACEBAR
        SPACE = 32,
        //
        // Summary:
        //     PAGE UP key
        PRIOR = 33,
        //
        // Summary:
        //     PAGE DOWN key
        NEXT = 34,
        //
        // Summary:
        //     END key
        END = 35,
        //
        // Summary:
        //     HOME key
        HOME = 36,
        //
        // Summary:
        //     LEFT ARROW key
        LEFT = 37,
        //
        // Summary:
        //     UP ARROW key
        UP = 38,
        //
        // Summary:
        //     RIGHT ARROW key
        RIGHT = 39,
        //
        // Summary:
        //     DOWN ARROW key
        DOWN = 40,
        //
        // Summary:
        //     SELECT key
        SELECT = 41,
        //
        // Summary:
        //     PRINT key
        PRINT = 42,
        //
        // Summary:
        //     EXECUTE key
        EXECUTE = 43,
        //
        // Summary:
        //     PRINT SCREEN key
        SNAPSHOT = 44,
        //
        // Summary:
        //     INS key
        INSERT = 45,
        //
        // Summary:
        //     DEL key
        DELETE = 46,
        //
        // Summary:
        //     HELP key
        HELP = 47,
        //
        // Summary:
        //     0 key
        KEY_0 = 48,
        //
        // Summary:
        //     1 key
        KEY_1 = 49,
        //
        // Summary:
        //     2 key
        KEY_2 = 50,
        //
        // Summary:
        //     3 key
        KEY_3 = 51,
        //
        // Summary:
        //     4 key
        KEY_4 = 52,
        //
        // Summary:
        //     5 key
        KEY_5 = 53,
        //
        // Summary:
        //     6 key
        KEY_6 = 54,
        //
        // Summary:
        //     7 key
        KEY_7 = 55,
        //
        // Summary:
        //     8 key
        KEY_8 = 56,
        //
        // Summary:
        //     9 key
        KEY_9 = 57,
        //
        // Summary:
        //     A key
        A = 65,
        //
        // Summary:
        //     B key
        B = 66,
        //
        // Summary:
        //     C key
        C = 67,
        //
        // Summary:
        //     D key
        D = 68,
        //
        // Summary:
        //     E key
        E = 69,
        //
        // Summary:
        //     F key
        F = 70,
        //
        // Summary:
        //     G key
        G = 71,
        //
        // Summary:
        //     H key
        H = 72,
        //
        // Summary:
        //     I key
        I = 73,
        //
        // Summary:
        //     J key
        J = 74,
        //
        // Summary:
        //     K key
        K = 75,
        //
        // Summary:
        //     L key
        L = 76,
        //
        // Summary:
        //     M key
        M = 77,
        //
        // Summary:
        //     N key
        N = 78,
        //
        // Summary:
        //     O key
        O = 79,
        //
        // Summary:
        //     P key
        P = 80,
        //
        // Summary:
        //     Q key
        Q = 81,
        //
        // Summary:
        //     R key
        R = 82,
        //
        // Summary:
        //     S key
        S = 83,
        //
        // Summary:
        //     T key
        T = 84,
        //
        // Summary:
        //     U key
        U = 85,
        //
        // Summary:
        //     V key
        V = 86,
        //
        // Summary:
        //     W key
        W = 87,
        //
        // Summary:
        //     X key
        X = 88,
        //
        // Summary:
        //     Y key
        Y = 89,
        //
        // Summary:
        //     Z key
        Z = 90,
        //
        // Summary:
        //     Left Windows key (Natural keyboard)
        LWIN = 91,
        //
        // Summary:
        //     Right Windows key (Natural keyboard)
        RWIN = 92,
        //
        // Summary:
        //     Applications key (Natural keyboard)
        APPS = 93,
        //
        // Summary:
        //     Computer Sleep key
        SLEEP = 95,
        //
        // Summary:
        //     Numeric keypad 0 key
        NUMPAD0 = 96,
        //
        // Summary:
        //     Numeric keypad 1 key
        NUMPAD1 = 97,
        //
        // Summary:
        //     Numeric keypad 2 key
        NUMPAD2 = 98,
        //
        // Summary:
        //     Numeric keypad 3 key
        NUMPAD3 = 99,
        //
        // Summary:
        //     Numeric keypad 4 key
        NUMPAD4 = 100,
        //
        // Summary:
        //     Numeric keypad 5 key
        NUMPAD5 = 101,
        //
        // Summary:
        //     Numeric keypad 6 key
        NUMPAD6 = 102,
        //
        // Summary:
        //     Numeric keypad 7 key
        NUMPAD7 = 103,
        //
        // Summary:
        //     Numeric keypad 8 key
        NUMPAD8 = 104,
        //
        // Summary:
        //     Numeric keypad 9 key
        NUMPAD9 = 105,
        //
        // Summary:
        //     Multiply key
        MULTIPLY = 106,
        //
        // Summary:
        //     Add key
        ADD = 107,
        //
        // Summary:
        //     Separator key
        SEPARATOR = 108,
        //
        // Summary:
        //     Subtract key
        SUBTRACT = 109,
        //
        // Summary:
        //     Decimal key
        DECIMAL = 110,
        //
        // Summary:
        //     Divide key
        DIVIDE = 111,
        //
        // Summary:
        //     F1 Key
        F1 = 112,
        //
        // Summary:
        //     F2 Key
        F2 = 113,
        //
        // Summary:
        //     F3 Key
        F3 = 114,
        //
        // Summary:
        //     F4 Key
        F4 = 115,
        //
        // Summary:
        //     F5 Key
        F5 = 116,
        //
        // Summary:
        //     F6 Key
        F6 = 117,
        //
        // Summary:
        //     F7 Key
        F7 = 118,
        //
        // Summary:
        //     F8 Key
        F8 = 119,
        //
        // Summary:
        //     F9 Key
        F9 = 120,
        //
        // Summary:
        //     F10 Key
        F10 = 121,
        //
        // Summary:
        //     F11 Key
        F11 = 122,
        //
        // Summary:
        //     F12 Key
        F12 = 123,
        //
        // Summary:
        //     F13 Key
        F13 = 124,
        //
        // Summary:
        //     F14 Key
        F14 = 125,
        //
        // Summary:
        //     F15 Key
        F15 = 126,
        //
        // Summary:
        //     F16 Key
        F16 = 127,
        //
        // Summary:
        //     F17 Key
        F17 = 128,
        //
        // Summary:
        //     F18 Key
        F18 = 129,
        //
        // Summary:
        //     F19 Key
        F19 = 130,
        //
        // Summary:
        //     F20 Key
        F20 = 131,
        //
        // Summary:
        //     F21 Key
        F21 = 132,
        //
        // Summary:
        //     F22 Key
        F22 = 133,
        //
        // Summary:
        //     F23 Key
        F23 = 134,
        //
        // Summary:
        //     F24 Key
        F24 = 135,
        //
        // Summary:
        //     NUM LOCK key
        NUMLOCK = 144,
        //
        // Summary:
        //     SCROLL LOCK key
        SCROLL = 145,
        //
        // Summary:
        //     '=' key on numpad (NEC PC-9800 kbd definitions)
        OEM_NEC_EQUAL = 146,
        //
        // Summary:
        //     'Dictionary' key (Fujitsu/OASYS kbd definitions)
        OEM_FJ_JISHO = 146,
        //
        // Summary:
        //     'Unregister word' key (Fujitsu/OASYS kbd definitions)
        OEM_FJ_MASSHOU = 147,
        //
        // Summary:
        //     'Register word' key (Fujitsu/OASYS kbd definitions)
        OEM_FJ_TOUROKU = 148,
        //
        // Summary:
        //     'Left OYAYUBI' key (Fujitsu/OASYS kbd definitions)
        OEM_FJ_LOYA = 149,
        //
        // Summary:
        //     'Right OYAYUBI' key (Fujitsu/OASYS kbd definitions)
        OEM_FJ_ROYA = 150,
        //
        // Summary:
        //     Left SHIFT key
        //
        // Remarks:
        //     Used only as parameters to PInvoke.User32.GetAsyncKeyState(System.Int32) and
        //     PInvoke.User32.GetKeyState(System.Int32). * No other API or message will distinguish
        //     left and right keys in this way.
        LSHIFT = 160,
        //
        // Summary:
        //     Right SHIFT key
        RSHIFT = 161,
        //
        // Summary:
        //     Left CONTROL key
        LCONTROL = 162,
        //
        // Summary:
        //     Right CONTROL key
        RCONTROL = 163,
        //
        // Summary:
        //     Left MENU key
        LMENU = 164,
        //
        // Summary:
        //     Right MENU key
        RMENU = 165,
        //
        // Summary:
        //     Browser Back key
        BROWSER_BACK = 166,
        //
        // Summary:
        //     Browser Forward key
        BROWSER_FORWARD = 167,
        //
        // Summary:
        //     Browser Refresh key
        BROWSER_REFRESH = 168,
        //
        // Summary:
        //     Browser Stop key
        BROWSER_STOP = 169,
        //
        // Summary:
        //     Browser Search key
        BROWSER_SEARCH = 170,
        //
        // Summary:
        //     Browser Favorites key
        BROWSER_FAVORITES = 171,
        //
        // Summary:
        //     Browser Start and Home key
        BROWSER_HOME = 172,
        //
        // Summary:
        //     Volume Mute key
        VOLUME_MUTE = 173,
        //
        // Summary:
        //     Volume Down key
        VOLUME_DOWN = 174,
        //
        // Summary:
        //     Volume Up key
        VOLUME_UP = 175,
        //
        // Summary:
        //     Next Track key
        MEDIA_NEXT_TRACK = 176,
        //
        // Summary:
        //     Previous Track key
        MEDIA_PREV_TRACK = 177,
        //
        // Summary:
        //     Stop Media key
        MEDIA_STOP = 178,
        //
        // Summary:
        //     Play/Pause Media key
        MEDIA_PLAY_PAUSE = 179,
        //
        // Summary:
        //     Start Mail key
        LAUNCH_MAIL = 180,
        //
        // Summary:
        //     Select Media key
        LAUNCH_MEDIA_SELECT = 181,
        //
        // Summary:
        //     Start Application 1 key
        LAUNCH_APP1 = 182,
        //
        // Summary:
        //     Start Application 2 key
        LAUNCH_APP2 = 183,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        //
        // Remarks:
        //     For the US standard keyboard, the ';:' key
        OEM_1 = 186,
        //
        // Summary:
        //     For any country/region, the '+' key.
        OEM_PLUS = 187,
        //
        // Summary:
        //     For any country/region, the ',' key.
        OEM_COMMA = 188,
        //
        // Summary:
        //     For any country/region, the '-' key.
        OEM_MINUS = 189,
        //
        // Summary:
        //     For any country/region, the '.' key.
        OEM_PERIOD = 190,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        //
        // Remarks:
        //     For the US standard keyboard, the '/?' key
        OEM_2 = 191,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        //
        // Remarks:
        //     For the US standard keyboard, the '`~' key
        OEM_3 = 192,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        //
        // Remarks:
        //     For the US standard keyboard, the '[{' key
        OEM_4 = 219,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        //
        // Remarks:
        //     For the US standard keyboard, the '\|' key
        OEM_5 = 220,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        //
        // Remarks:
        //     For the US standard keyboard, the ']}' key
        OEM_6 = 221,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        //
        // Remarks:
        //     For the US standard keyboard, the 'single-quote/double-quote' (''"') key
        OEM_7 = 222,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        OEM_8 = 223,
        //
        // Summary:
        //     OEM specific
        //
        // Remarks:
        //     'AX' key on Japanese AX kbd
        OEM_AX = 225,
        //
        // Summary:
        //     Either the angle bracket ("") key or the backslash ("\|") key on the RT 102-key
        //     keyboard
        OEM_102 = 226,
        //
        // Summary:
        //     OEM specific
        //
        // Remarks:
        //     Help key on ICO
        ICO_HELP = 227,
        //
        // Summary:
        //     OEM specific
        //
        // Remarks:
        //     00 key on ICO
        ICO_00 = 228,
        //
        // Summary:
        //     IME PROCESS key
        PROCESSKEY = 229,
        //
        // Summary:
        //     OEM specific
        //
        // Remarks:
        //     Clear key on ICO
        ICO_CLEAR = 230,
        //
        // Summary:
        //     Used to pass Unicode characters as if they were keystrokes. The PACKET key
        //     is the low word of a 32-bit Virtual Key value used for non-keyboard input methods.
        //
        // Remarks:
        //     For more information, see Remark in PInvoke.User32.KEYBDINPUT, PInvoke.User32.SendInput(System.Int32,PInvoke.User32.INPUT*,System.Int32),
        //     PInvoke.User32.WindowMessage.WM_KEYDOWN, and PInvoke.User32.WindowMessage.WM_KEYUP
        PACKET = 231,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_RESET = 233,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_JUMP = 234,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_PA1 = 235,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_PA2 = 236,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_PA3 = 237,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_WSCTRL = 238,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_CUSEL = 239,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_ATTN = 240,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_FINISH = 241,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_COPY = 242,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_AUTO = 243,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_ENLW = 244,
        //
        // Summary:
        //     Nokia/Ericsson definition
        OEM_BACKTAB = 245,
        //
        // Summary:
        //     Attn key
        ATTN = 246,
        //
        // Summary:
        //     CrSel key
        CRSEL = 247,
        //
        // Summary:
        //     ExSel key
        EXSEL = 248,
        //
        // Summary:
        //     Erase EOF key
        EREOF = 249,
        //
        // Summary:
        //     Play key
        PLAY = 250,
        //
        // Summary:
        //     Zoom key
        ZOOM = 251,
        //
        // Summary:
        //     Reserved constant by Windows headers definition
        NONAME = 252,
        //
        // Summary:
        //     PA1 key
        PA1 = 253,
        //
        // Summary:
        //     Clear key
        OEM_CLEAR = 254
    }
}
