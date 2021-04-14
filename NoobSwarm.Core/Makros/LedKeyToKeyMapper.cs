using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vulcan.NET;
using Key = NoobSwarm.Makros.Key;

namespace NoobSwarm.Makros
{
    public static class LedKeyToKeyMapper
    {
        public static IReadOnlyDictionary<LedKey, Key> LedKeyToKey { get; } = new Dictionary<LedKey, Key>()
        {
            {LedKey.ESC ,Key.ESCAPE },
            {LedKey.TILDE ,Key.OEM_5},
            {LedKey.TAB ,Key.TAB},
            {LedKey.CAPS_LOCK ,Key.CAPITAL},
            //{LedKey.EASY_SHIFT ,Key.NO_KEY},
            {LedKey.LEFT_SHIFT ,Key.LSHIFT},
            {LedKey.LEFT_CONTROL ,Key.LCONTROL},
            {LedKey.D1 ,Key.KEY_1},
            {LedKey.Q, Key.Q},
            {LedKey.A, Key.A},
            {LedKey.ISO_BACKSLASH ,Key.OEM_102},
            {LedKey.LEFT_WINDOWS ,Key.LWIN},
            {LedKey.F1, Key.F1},
            {LedKey.D2, Key.KEY_2},
            {LedKey.W, Key.W},
            {LedKey.S, Key.S},
            {LedKey.Y, Key.Y},
            {LedKey.LEFT_ALT ,Key.LMENU},
            {LedKey.F2, Key.F2},
            {LedKey.D3, Key.KEY_3 },
            {LedKey.E, Key.E},
            {LedKey.D, Key.D},
            {LedKey.X, Key.X},
            {LedKey.F3, Key.F3},
            {LedKey.D4, Key.KEY_4},
            {LedKey.R, Key.R},
            {LedKey.F, Key.F},
            {LedKey.C, Key.C},
            {LedKey.F4, Key.F4},
            {LedKey.D5, Key.KEY_5},
            {LedKey.T, Key.T},
            {LedKey.G, Key.G},
            {LedKey.V, Key.V},
            {LedKey.D6, Key.KEY_6},
            {LedKey.Z, Key.Z},
            {LedKey.H, Key.H},
            {LedKey.B, Key.B},
            {LedKey.SPACE ,Key.SPACE},
            {LedKey.F5, Key.F5},
            {LedKey.D7, Key.KEY_7},
            {LedKey.U, Key.U},
            {LedKey.J, Key.J},
            {LedKey.N, Key.N},
            {LedKey.F6, Key.F6},
            {LedKey.D8, Key.KEY_8},
            {LedKey.I, Key.I},
            {LedKey.K, Key.K},
            {LedKey.M, Key.M},
            {LedKey.F7, Key.F7},
            {LedKey.D9, Key.KEY_9},
            {LedKey.O, Key.O},
            {LedKey.L, Key.L},
            {LedKey.OEMCOMMA ,Key.OEM_COMMA},
            {LedKey.F8, Key.F8},
            {LedKey.D0, Key.KEY_0},
            {LedKey.P, Key.P},
            {LedKey.SEMICOLON ,Key.OEM_3},
            {LedKey.OEMPERIOD ,Key.OEM_PERIOD},
            {LedKey.RIGHT_ALT ,Key.RMENU},
            {LedKey.OEMMINUS ,Key.OEM_4},
            {LedKey.OPEN_BRACKET ,Key.OEM_1},
            {LedKey.APOSTROPHE ,Key.OEM_7},
            {LedKey.FORWARD_SLASH ,Key.OEM_MINUS},
            {LedKey.FN_Key ,Key.NO_KEY},
            {LedKey.F9, Key.F9},
            {LedKey.EQUALS ,Key.OEM_6},
            {LedKey.CLOSE_BRACKET ,Key.OEM_PLUS},
            {LedKey.BACKSLASH ,Key.NO_KEY},
            {LedKey.RIGHT_SHIFT ,Key.RSHIFT},
            {LedKey.APPLICATION_SELECT ,Key.APPS},
            {LedKey.F10, Key.F10},
            {LedKey.F11, Key.F11},
            {LedKey.F12, Key.F12},
            {LedKey.BACKSPACE ,Key.BACK},
            {LedKey.ENTER ,Key.RETURN},
            {LedKey.RIGHT_CONTROL ,Key.RCONTROL},
            {LedKey.ISO_HASH ,Key.OEM_2},
            {LedKey.PRINT_SCREEN ,Key.SNAPSHOT},
            {LedKey.INSERT ,Key.INSERT},
            {LedKey.DELETE ,Key.DELETE},
            {LedKey.ARROW_LEFT ,Key.LEFT},
            {LedKey.SCROLL_LOCK ,Key.SCROLL},
            {LedKey.HOME ,Key.HOME},
            {LedKey.END ,Key.END},
            {LedKey.ARROW_UP ,Key.UP},
            {LedKey.ARROW_DOWN ,Key.DOWN},
            {LedKey.PAUSE_BREAK ,Key.PAUSE},
            {LedKey.PAGE_UP ,Key.PRIOR},
            {LedKey.PAGE_DOWN ,Key.NEXT},
            {LedKey.ARROW_RIGHT ,Key.RIGHT},
            {LedKey.NUM_LOCK ,Key.NUMLOCK},
            {LedKey.NUMPAD7, Key.NUMPAD7},
            {LedKey.NUMPAD4, Key.NUMPAD4},
            {LedKey.NUMPAD1, Key.NUMPAD1},
            {LedKey.NUMPAD0, Key.NUMPAD0},
            {LedKey.DIVIDE ,Key.DIVIDE},
            {LedKey.NUMPAD8, Key.NUMPAD8},
            {LedKey.NUMPAD5, Key.NUMPAD5},
            {LedKey.NUMPAD2, Key.NUMPAD2},
            {LedKey.MULTIPLY ,Key.MULTIPLY},
            {LedKey.NUMPAD9, Key.NUMPAD9},
            {LedKey.NUMPAD6, Key.NUMPAD6},
            {LedKey.NUMPAD3, Key.NUMPAD3},
            {LedKey.DECIMAL ,Key.DECIMAL},
            {LedKey.SUBTRACT ,Key.SUBTRACT},
            {LedKey.ADD ,Key.ADD},
            {LedKey.NUM_ENTER ,Key.RETURN}
        };
        public static IReadOnlyDictionary<Key, LedKey> KeyToLedKey { get; }

        static LedKeyToKeyMapper()
        {
            var keyToLedKey = new Dictionary<Key, LedKey>();
            foreach (var item in LedKeyToKey)
            {
                if (item.Value == Key.NO_KEY)
                    continue;

                keyToLedKey.TryAdd(item.Value, item.Key);
            }
            KeyToLedKey = keyToLedKey;

        }
    }
}
