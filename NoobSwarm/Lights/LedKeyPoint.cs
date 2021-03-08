
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights
{

    public struct LedKeyPoint
    {
        public LedKeyPoint(int x, int y, LedKey ledKey)
        {
            X = x;
            Y = y;
            LedKey = ledKey;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public LedKey LedKey { get; set; }

        public static IReadOnlyList<LedKeyPoint> LedKeyPoints = new List<LedKeyPoint>() {
            new LedKeyPoint(0, 0, LedKey.ESC),
            new LedKeyPoint(90, 0, LedKey.F1),
            new LedKeyPoint(135, 0, LedKey.F2),
            new LedKeyPoint(179, 0, LedKey.F3),
            new LedKeyPoint(221, 0, LedKey.F4),
            new LedKeyPoint(290, 0, LedKey.F5),
            new LedKeyPoint(334, 0, LedKey.F6),
            new LedKeyPoint(378, 0, LedKey.F7),
            new LedKeyPoint(423, 0, LedKey.F8),
            new LedKeyPoint(490, 0, LedKey.F9),
            new LedKeyPoint(534, 0, LedKey.F10),
            new LedKeyPoint(582, 0, LedKey.F11),
            new LedKeyPoint(623, 0, LedKey.F12),
            new LedKeyPoint(680, 0, LedKey.PRINT_SCREEN),
            new LedKeyPoint(724, 0, LedKey.SCROLL_LOCK),
            new LedKeyPoint(768, 0, LedKey.PAUSE_BREAK),
            new LedKeyPoint(0, 60, LedKey.TILDE),
            new LedKeyPoint(47, 60, LedKey.D1),
            new LedKeyPoint(90, 60, LedKey.D2),
            new LedKeyPoint(135, 60, LedKey.D3),
            new LedKeyPoint(179, 60, LedKey.D4),
            new LedKeyPoint(225, 60, LedKey.D5),
            new LedKeyPoint(267, 60, LedKey.D6),
            new LedKeyPoint(313, 60, LedKey.D7),
            new LedKeyPoint(357, 60, LedKey.D8),
            new LedKeyPoint(400, 60, LedKey.D9),
            new LedKeyPoint(448, 60, LedKey.D0),
            new LedKeyPoint(490, 60, LedKey.OEMMINUS),
            new LedKeyPoint(534, 60, LedKey.EQUALS),
            new LedKeyPoint(604, 60, LedKey.BACKSPACE),
            new LedKeyPoint(680, 60, LedKey.INSERT),
            new LedKeyPoint(724, 60, LedKey.HOME),
            new LedKeyPoint(768, 60, LedKey.PAGE_UP),
            new LedKeyPoint(826, 60, LedKey.NUM_LOCK),
            new LedKeyPoint(870, 60, LedKey.DIVIDE),
            new LedKeyPoint(914, 60, LedKey.MULTIPLY),
            new LedKeyPoint(958, 60, LedKey.SUBTRACT),
            new LedKeyPoint(68, 100, LedKey.Q),
            new LedKeyPoint(114, 100, LedKey.W),
            new LedKeyPoint(157, 100, LedKey.E),
            new LedKeyPoint(202, 100, LedKey.R),
            new LedKeyPoint(245, 100, LedKey.T),
            new LedKeyPoint(290, 100, LedKey.Z),
            new LedKeyPoint(334, 100, LedKey.U),
            new LedKeyPoint(379, 100, LedKey.I),
            new LedKeyPoint(423, 100, LedKey.O),
            new LedKeyPoint(468, 100, LedKey.P),
            new LedKeyPoint(512, 100, LedKey.OPEN_BRACKET),
            new LedKeyPoint(556, 100, LedKey.CLOSE_BRACKET),
            new LedKeyPoint(680, 100, LedKey.DELETE),
            new LedKeyPoint(724, 100, LedKey.END),
            new LedKeyPoint(772, 100, LedKey.PAGE_DOWN),
            new LedKeyPoint(826, 100, LedKey.NUMPAD7),
            new LedKeyPoint(870, 100, LedKey.NUMPAD8),
            new LedKeyPoint(914, 100, LedKey.NUMPAD9),
            new LedKeyPoint(14, 103, LedKey.TAB),
            new LedKeyPoint(618, 115, LedKey.ENTER),
            new LedKeyPoint(958, 115, LedKey.ADD),
            new LedKeyPoint(79, 146, LedKey.A),
            new LedKeyPoint(125, 146, LedKey.S),
            new LedKeyPoint(168, 146, LedKey.D),
            new LedKeyPoint(213, 146, LedKey.F),
            new LedKeyPoint(256, 146, LedKey.G),
            new LedKeyPoint(302, 146, LedKey.H),
            new LedKeyPoint(346, 146, LedKey.J),
            new LedKeyPoint(391, 146, LedKey.K),
            new LedKeyPoint(435, 146, LedKey.L),
            new LedKeyPoint(480, 146, LedKey.SEMICOLON),
            new LedKeyPoint(523, 146, LedKey.APOSTROPHE),
            new LedKeyPoint(566, 146, LedKey.ISO_HASH),
            new LedKeyPoint(826, 146, LedKey.NUMPAD4),
            new LedKeyPoint(870, 146, LedKey.NUMPAD5),
            new LedKeyPoint(914, 146, LedKey.NUMPAD6),
            new LedKeyPoint(19, 148, LedKey.CAPS_LOCK),
            new LedKeyPoint(0, 192, LedKey.LEFT_SHIFT),
            new LedKeyPoint(56, 192, LedKey.ISO_BACKSLASH),
            new LedKeyPoint(100, 192, LedKey.Y),
            new LedKeyPoint(146, 192, LedKey.X),
            new LedKeyPoint(190, 192, LedKey.C),
            new LedKeyPoint(234, 192, LedKey.V),
            new LedKeyPoint(280, 192, LedKey.B),
            new LedKeyPoint(324, 192, LedKey.N),
            new LedKeyPoint(368, 192, LedKey.M),
            new LedKeyPoint(412, 192, LedKey.OEMCOMMA),
            new LedKeyPoint(456, 192, LedKey.OEMPERIOD),
            new LedKeyPoint(500, 192, LedKey.FORWARD_SLASH),
            new LedKeyPoint(586, 192, LedKey.RIGHT_SHIFT),
            new LedKeyPoint(724, 192, LedKey.ARROW_UP),
            new LedKeyPoint(826, 192, LedKey.NUMPAD1),
            new LedKeyPoint(870, 192, LedKey.NUMPAD2),
            new LedKeyPoint(914, 192, LedKey.NUMPAD3),
            new LedKeyPoint(958, 206, LedKey.NUM_ENTER),
            new LedKeyPoint(0, 233, LedKey.LEFT_CONTROL),
            new LedKeyPoint(64, 234, LedKey.LEFT_WINDOWS),
            new LedKeyPoint(121, 234, LedKey.LEFT_ALT),
            new LedKeyPoint(284, 234, LedKey.SPACE),
            new LedKeyPoint(453, 234, LedKey.RIGHT_ALT),
            new LedKeyPoint(509, 234, LedKey.FN_Key),
            new LedKeyPoint(562, 234, LedKey.APPLICATION_SELECT),
            new LedKeyPoint(618, 234, LedKey.RIGHT_CONTROL),
            new LedKeyPoint(680, 234, LedKey.ARROW_LEFT),
            new LedKeyPoint(724, 234, LedKey.ARROW_DOWN),
            new LedKeyPoint(772, 234, LedKey.ARROW_RIGHT),
            new LedKeyPoint(849, 234, LedKey.NUMPAD0),
            new LedKeyPoint(914, 234, LedKey.DECIMAL)
        };

    }
}
