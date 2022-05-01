using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Vulcan.NET;
using NoobSwarm.Hid.Reports.Input;
using NoobSwarm.Hid;
using System.Threading;
using System.Buffers;
using NoobSwarm.Lights;
using System.Diagnostics;

namespace NoobSwarm.GenericKeyboard
{
    public sealed class QMKKeyboard : IVulcanKeyboard, IDisposable, ILedKeyPointProvider
    {
        public event EventHandler<TestArgs> TestKeyPressedReceived;
        public event EventHandler<KeyPressedArgs> KeyPressedReceived;
        public event EventHandler<VolumeKnobArgs> VolumeKnobPressedReceived;
        public event EventHandler<VolumeKnobFxArgs> VolumeKnobFxPressedReceived;
        public event EventHandler<VolumeKnDirectionArgs> VolumeKnobTurnedReceived;
        public event EventHandler<VolumeKnDirectionArgs> DPITurnedReceived;


        enum MainModes
        {
            RGB_COMMAND,
            LAYER_COMMAND,
            MACRO_COMMAND,
            OTHER_COMMAND
        };

        enum RGBModes
        {

            IndexItereationRGBZero,
            Reserved,
            PerKeyRGB,
            IndexItereationRGB,
        };
        enum LayerModes
        {
            SwitchToLayer,
            ResetLayer,
        };
        enum MacroModes
        {
            PCMacro,
            PCMacro16Bit,
            PCMacroString
        };

        enum OtherModes
        {
            CustomKeyCode,
            CustomCommands = 255
        };

        enum custom_commands
        {
            ResetKeyboard = 254,
            GoIntoBootloader = 255
        };

        public byte Brightness
        {
            get => brightness; set
            {
                if (value == brightness)
                    return;
                brightness = Math.Clamp((byte)value, (byte)0, (byte)69);
                //_ = Update().GetAwaiter().GetResult();
            }
        }

        private byte ReportId => reportId++;

        private byte brightness = 69;

        private byte reportId = 0;
        private HidStream _ctrlStream;
        private CancellationTokenSource _source;
        private Thread updateOperationThread;
        private readonly byte[] _keyColors = new byte[303];
        private readonly List<HidStream> streamsToDispose = new List<HidStream>();
        private readonly AutoResetEvent updateOperationResetEvent = new(false);

        private UpdateOperation updateOperation;

        private QMKKeyboard(HidStream ctrlStream)
        {
            _ctrlStream = ctrlStream;
            _source = new CancellationTokenSource();

            updateOperationThread = new Thread(() => UpdateThread(_source.Token))
            {
                IsBackground = true
            };
            updateOperationThread.Start();
        }

        const int RAW_EPSIZE = 64;

        private static readonly Dictionary<MyKeycodes, LedKey> MyKeycodesToLedKeyMapping = new Dictionary<MyKeycodes, LedKey>()
        {
            { MyKeycodes.MYCKC_ESC, LedKey.ESC },
            { MyKeycodes.MYCKC_GRV, LedKey.TILDE },
            { MyKeycodes.MYCKC_TAB, LedKey.TAB },
            { MyKeycodes.MYCKC_LSFT, LedKey.LEFT_SHIFT },
            { MyKeycodes.MYCKC_LCTL, LedKey.LEFT_CONTROL },
            { MyKeycodes.MYCKC_1, LedKey.D1 },
            { MyKeycodes.MYCKC_Q, LedKey.Q },
            { MyKeycodes.MYCKC_A, LedKey.A },
            { MyKeycodes.MYCKC_NUBS, LedKey.ISO_BACKSLASH },
            { MyKeycodes.MYCKC_LGUI, LedKey.LEFT_WINDOWS },
            { MyKeycodes.MYCKC_F1, LedKey.F1 },
            { MyKeycodes.MYCKC_2, LedKey.D2 },
            { MyKeycodes.MYCKC_W, LedKey.W },
            { MyKeycodes.MYCKC_S, LedKey.S },
            { MyKeycodes.MYCKC_Z, LedKey.Z },
            { MyKeycodes.MYCKC_LALT, LedKey.LEFT_ALT },
            { MyKeycodes.MYCKC_F2, LedKey.F2 },
            { MyKeycodes.MYCKC_3, LedKey.D3 },
            { MyKeycodes.MYCKC_E, LedKey.E },
            { MyKeycodes.MYCKC_D, LedKey.D },
            { MyKeycodes.MYCKC_X, LedKey.X },
            { MyKeycodes.MYCKC_F3, LedKey.F3 },
            { MyKeycodes.MYCKC_4, LedKey.D4 },
            { MyKeycodes.MYCKC_R, LedKey.R },
            { MyKeycodes.MYCKC_F , LedKey.F },
            { MyKeycodes.MYCKC_C, LedKey.C },
            { MyKeycodes.MYCKC_F4, LedKey.F4 },
            { MyKeycodes.MYCKC_5, LedKey.D5 },
            { MyKeycodes.MYCKC_T, LedKey.T },
            { MyKeycodes.MYCKC_G, LedKey.G },
            { MyKeycodes.MYCKC_V, LedKey.V },
            { MyKeycodes.MYCKC_6, LedKey.D6 },
            { MyKeycodes.MYCKC_Y, LedKey.Y },
            { MyKeycodes.MYCKC_H, LedKey.H },
            { MyKeycodes.MYCKC_B, LedKey.B },
            { MyKeycodes.MYCKC_SPC, LedKey.SPACE },
            { MyKeycodes.MYCKC_F5, LedKey.F5 },
            { MyKeycodes.MYCKC_7, LedKey.D7 },
            { MyKeycodes.MYCKC_U, LedKey.U },
            { MyKeycodes.MYCKC_J, LedKey.J },
            { MyKeycodes.MYCKC_N, LedKey.N },
            { MyKeycodes.MYCKC_F6, LedKey.F6 },
            { MyKeycodes.MYCKC_8, LedKey.D8 },
            { MyKeycodes.MYCKC_I, LedKey.I },
            { MyKeycodes.MYCKC_K, LedKey.K },
            { MyKeycodes.MYCKC_M, LedKey.M },
            { MyKeycodes.MYCKC_F7, LedKey.F7 },
            { MyKeycodes.MYCKC_9, LedKey.D9 },
            { MyKeycodes.MYCKC_O, LedKey.O },
            { MyKeycodes.MYCKC_L, LedKey.L },
            { MyKeycodes.MYCKC_COMM, LedKey.OEMCOMMA },
            { MyKeycodes.MYCKC_F8, LedKey.F8 },
            { MyKeycodes.MYCKC_0, LedKey.D0 },
            { MyKeycodes.MYCKC_P, LedKey.P },
            { MyKeycodes.MYCKC_SCLN, LedKey.SEMICOLON },
            { MyKeycodes.MYCKC_DOT, LedKey.OEMPERIOD },
            { MyKeycodes.MYCKC_RALT, LedKey.RIGHT_ALT },
            { MyKeycodes.MYCKC_MINS, LedKey.OEMMINUS },
            { MyKeycodes.MYCKC_LBRC, LedKey.OPEN_BRACKET },
            { MyKeycodes.MYCKC_QUOT, LedKey.APOSTROPHE },
            { MyKeycodes.MYCKC_SLSH, LedKey.FORWARD_SLASH },
            { MyKeycodes.MYCKC_FN, LedKey.FN_Key },
            { MyKeycodes.MYCKC_F9, LedKey.F9 },
            { MyKeycodes.MYCKC_EQL, LedKey.EQUALS },
            { MyKeycodes.MYCKC_RBRC, LedKey.CLOSE_BRACKET },
            { MyKeycodes.MYCKC_F10, LedKey.F10 },
            { MyKeycodes.MYCKC_F11, LedKey.F11 },
            { MyKeycodes.MYCKC_F12, LedKey.F12 },
            { MyKeycodes.MYCKC_BSPC, LedKey.BACKSPACE },
            { MyKeycodes.MYCKC_ENT, LedKey.ENTER },
            { MyKeycodes.MYCKC_RCTL, LedKey.RIGHT_CONTROL },
            { MyKeycodes.MYCKC_NUHS, LedKey.ISO_HASH },
            { MyKeycodes.MYCKC_DEL, LedKey.DELETE },
            { MyKeycodes.MYCKC_LEFT, LedKey.ARROW_LEFT },
            { MyKeycodes.MYCKC_HOME, LedKey.HOME },
            { MyKeycodes.MYCKC_END, LedKey.END },
            { MyKeycodes.MYCKC_UP, LedKey.ARROW_UP },
            { MyKeycodes.MYCKC_DOWN, LedKey.ARROW_DOWN },
            { MyKeycodes.MYCKC_PGUP, LedKey.PAGE_UP },
            { MyKeycodes.MYCKC_PGDN, LedKey.PAGE_DOWN },
            { MyKeycodes.MYCKC_RGHT, LedKey.ARROW_RIGHT },
            { MyKeycodes.MYCKC_NLCK, LedKey.NUM_LOCK },
            { MyKeycodes.MYCKC_P7, LedKey.NUMPAD7 },
            { MyKeycodes.MYCKC_P4, LedKey.NUMPAD4 },
            { MyKeycodes.MYCKC_P1, LedKey.NUMPAD1 },
            { MyKeycodes.MYCKC_P0, LedKey.NUMPAD0 },
            { MyKeycodes.MYCKC_PSLS, LedKey.DIVIDE },
            { MyKeycodes.MYCKC_P8, LedKey.NUMPAD8 },
            { MyKeycodes.MYCKC_P5, LedKey.NUMPAD5 },
            { MyKeycodes.MYCKC_P2, LedKey.NUMPAD2 },
            { MyKeycodes.MYCKC_PAST, LedKey.MULTIPLY },
            { MyKeycodes.MYCKC_P9, LedKey.NUMPAD9 },
            { MyKeycodes.MYCKC_P6, LedKey.NUMPAD6 },
            { MyKeycodes.MYCKC_P3, LedKey.NUMPAD3 },
            { MyKeycodes.MYCKC_NUMCOL, LedKey.DECIMAL },
            { MyKeycodes.MYCKC_PMNS, LedKey.SUBTRACT },
            { MyKeycodes.MYCKC_PPLS, LedKey.ADD },
            { MyKeycodes.MYCKC_PENT, LedKey.NUM_ENTER },
        };

        private static readonly Dictionary<(byte, byte), LedKey> RowColumnToLedKeyMapping = new Dictionary<(byte, byte), LedKey>()
        {

            {(0, 0), LedKey.ESC },
            {(0, 1), LedKey.TILDE},
            {(0, 2), LedKey.TAB},
            {(0, 3), LedKey.EASY_SHIFT},
            {(0, 4), LedKey.LEFT_SHIFT},
            {(0, 5), LedKey.LEFT_CONTROL},
            {(1, 0), LedKey.F1},
            {(1, 1), LedKey.D1},
            {(1, 2), LedKey.Q},
            {(1, 3), LedKey.A},
            {(1, 4), LedKey.ISO_BACKSLASH},
            {(1, 5), LedKey.LEFT_WINDOWS},
            {(2, 0), LedKey.F2},
            {(2, 1), LedKey.D2},
            {(2, 2), LedKey.W},
            {(2, 3), LedKey.S},
            {(2, 4), LedKey.Z},
            {(2, 5), LedKey.LEFT_ALT},
            {(3, 0), LedKey.F3},
            {(3, 1), LedKey.D3},
            {(3, 2), LedKey.E},
            {(3, 3), LedKey.D},
            {(3, 4), LedKey.X},
            {(4, 0), LedKey.F4},
            {(4, 1), LedKey.D4},
            {(4, 2), LedKey.R},
            {(4, 3), LedKey.F},
            {(4, 4), LedKey.C},
            {(5, 0), LedKey.F5},
            {(5, 1), LedKey.D5},
            {(5, 2), LedKey.T},
            {(5, 3), LedKey.G},
            {(5, 4), LedKey.V},
            {(6, 0), LedKey.F6},
            {(6, 1), LedKey.D6},
            {(6, 2), LedKey.Y},
            {(6, 3), LedKey.H},
            {(6, 4), LedKey.B},
            {(6, 5), LedKey.SPACE},
            {(7, 0), LedKey.F7},
            {(7, 1), LedKey.D7},
            {(7, 2), LedKey.U},
            {(7, 3), LedKey.J},
            {(7, 4), LedKey.N},
            {(8, 0), LedKey.F8},
            {(8, 1), LedKey.D8},
            {(8, 2), LedKey.I},
            {(8, 3), LedKey.K},
            {(8, 4), LedKey.M},
            {(9, 0), LedKey.F9},
            {(9, 1), LedKey.D9},
            {(9, 2), LedKey.O},
            {(9, 3), LedKey.L},
            {(9, 4), LedKey.OEMCOMMA},
            {(10, 0), LedKey.F10},
            {(10, 1), LedKey.D0},
            {(10, 2), LedKey.P},
            {(10, 3), LedKey.SEMICOLON},
            {(10, 4), LedKey.OEMPERIOD},
            {(10, 5), LedKey.RIGHT_ALT},
            {(11, 0), LedKey.F11},
            {(11, 1), LedKey.OEMMINUS},
            {(11, 2), LedKey.OPEN_BRACKET},
            {(11, 3), LedKey.APOSTROPHE},
            {(11, 4), LedKey.FORWARD_SLASH},
            {(11, 5), LedKey.FN_Key},
            {(12, 0), LedKey.F12},
            {(12, 1), LedKey.EQUALS},
            {(12, 2), LedKey.CLOSE_BRACKET},
            {(12, 3), LedKey.ISO_HASH},
            {(12, 5), LedKey.RIGHT_CONTROL},
            {(13, 0), LedKey.DELETE},
            {(13, 1), LedKey.BACKSPACE},
            {(13, 3), LedKey.ENTER},
            {(13, 5), LedKey.ARROW_LEFT},
            {(14, 0), LedKey.HOME},
            {(14, 4), LedKey.ARROW_UP},
            {(14, 5), LedKey.ARROW_DOWN},
            {(15, 0), LedKey.END},
            {(15, 1), LedKey.NUM_LOCK},
            {(15, 2), LedKey.NUMPAD7},
            {(15, 3), LedKey.NUMPAD4},
            {(15, 4), LedKey.NUMPAD1},
            {(15, 5), LedKey.ARROW_RIGHT},
            {(16, 0), LedKey.PAGE_UP},
            {(16, 1), LedKey.DIVIDE},
            {(16, 2), LedKey.NUMPAD8},
            {(16, 3), LedKey.NUMPAD5},
            {(16, 4), LedKey.NUMPAD2},
            {(16, 5), LedKey.NUMPAD0},
            {(17, 0), LedKey.PAGE_DOWN},
            {(17, 1), LedKey.MULTIPLY},
            {(17, 2), LedKey.NUMPAD9},
            {(17, 3), LedKey.NUMPAD6},
            {(17, 4), LedKey.NUMPAD3},
            {(17, 5), LedKey.DECIMAL},
            {(18, 0), LedKey.PRINT_SCREEN},
            {(18, 1), LedKey.SUBTRACT},
            {(18, 2), LedKey.ADD},
            {(18, 4), LedKey.NUM_ENTER},

    };
        private static readonly List<LedKey> LedKeyIndicesMapping = new List<LedKey>()
        {
            LedKey.ESC,
            LedKey.F1,
            LedKey.F2,
            LedKey.F3,
            LedKey.F4,
            LedKey.F5,
            LedKey.F6,
            LedKey.F7,
            LedKey.F8,
            LedKey.F9,
            LedKey.F10,
            LedKey.F11,
            LedKey.F12,
            LedKey.DELETE,
            LedKey.HOME,
            LedKey.END,
            LedKey.PAGE_UP,
            LedKey.PAGE_DOWN,
            LedKey.PRINT_SCREEN,
            LedKey.TILDE,
            LedKey.D1,
            LedKey.D2,
            LedKey.D3,
            LedKey.D4,
            LedKey.D5,
            LedKey.D6,
            LedKey.D7,
            LedKey.D8,
            LedKey.D9,
            LedKey.D0,
            LedKey.OEMMINUS,
            LedKey.EQUALS,
            LedKey.BACKSPACE,
            LedKey.NUM_LOCK,
            LedKey.DIVIDE,
            LedKey.MULTIPLY,
            LedKey.SUBTRACT,
            LedKey.TAB,
            LedKey.Q,
            LedKey.W,
            LedKey.E,
            LedKey.R,
            LedKey.T,
            LedKey.Y,
            LedKey.U,
            LedKey.I,
            LedKey.O,
            LedKey.P,
            LedKey.OPEN_BRACKET,
            LedKey.CLOSE_BRACKET,
            LedKey.NUMPAD7,
            LedKey.NUMPAD8,
            LedKey.NUMPAD9,
            LedKey.ADD,
            LedKey.EASY_SHIFT,
            LedKey.A,
            LedKey.S,
            LedKey.D,
            LedKey.F,
            LedKey.G,
            LedKey.H,
            LedKey.J,
            LedKey.K,
            LedKey.L,
            LedKey.SEMICOLON,
            LedKey.APOSTROPHE,
            LedKey.ISO_HASH,
            LedKey.ENTER,
            LedKey.NUMPAD4,
            LedKey.NUMPAD5,
            LedKey.NUMPAD6,
            LedKey.LEFT_SHIFT,
            LedKey.ISO_BACKSLASH,
            LedKey.Z,
            LedKey.X,
            LedKey.C,
            LedKey.V,
            LedKey.B,
            LedKey.N,
            LedKey.M,
            LedKey.OEMCOMMA,
            LedKey.OEMPERIOD,
            LedKey.FORWARD_SLASH,
            LedKey.RIGHT_SHIFT,
            LedKey.ARROW_UP,
            LedKey.NUMPAD1,
            LedKey.NUMPAD2,
            LedKey.NUMPAD3,
            LedKey.NUM_ENTER,
            LedKey.LEFT_CONTROL,
            LedKey.LEFT_WINDOWS,
            LedKey.LEFT_ALT,
            LedKey.SPACE,
            LedKey.RIGHT_ALT,
            LedKey.FN_Key,
            LedKey.RIGHT_CONTROL,
            LedKey.ARROW_LEFT,
            LedKey.ARROW_DOWN,
            LedKey.ARROW_RIGHT,
            LedKey.NUMPAD0,
            LedKey.DECIMAL,
        };

        private static IReadOnlyList<LedKeyPoint> LedKeyPoints = new List<LedKeyPoint>() {
            new LedKeyPoint(0, 0, LedKey.ESC),
            new LedKeyPoint(47, 0, LedKey.F1),
            new LedKeyPoint(94, 0, LedKey.F2),
            new LedKeyPoint(141, 0, LedKey.F3),
            new LedKeyPoint(188, 0, LedKey.F4),
            new LedKeyPoint(235, 0, LedKey.F5),
            new LedKeyPoint(282, 0, LedKey.F6),
            new LedKeyPoint(329, 0, LedKey.F7),
            new LedKeyPoint(376, 0, LedKey.F8),
            new LedKeyPoint(423, 0, LedKey.F9),
            new LedKeyPoint(470, 0, LedKey.F10),
            new LedKeyPoint(517, 0, LedKey.F11),
            new LedKeyPoint(564, 0, LedKey.F12),
            new LedKeyPoint(611, 0, LedKey.DELETE),
            new LedKeyPoint(658, 0, LedKey.HOME),
            new LedKeyPoint(705, 0, LedKey.END),
            new LedKeyPoint(752, 0, LedKey.PAGE_UP),
            new LedKeyPoint(799, 0, LedKey.PAGE_DOWN),
            new LedKeyPoint(846, 0, LedKey.PRINT_SCREEN),
            new LedKeyPoint(0, 47, LedKey.TILDE),
            new LedKeyPoint(47, 47, LedKey.D1),
            new LedKeyPoint(94, 47, LedKey.D2),
            new LedKeyPoint(141, 47, LedKey.D3),
            new LedKeyPoint(188, 47, LedKey.D4),
            new LedKeyPoint(235, 47, LedKey.D5),
            new LedKeyPoint(282, 47, LedKey.D6),
            new LedKeyPoint(329, 47, LedKey.D7),
            new LedKeyPoint(376, 47, LedKey.D8),
            new LedKeyPoint(423, 47, LedKey.D9),
            new LedKeyPoint(470, 47, LedKey.D0),
            new LedKeyPoint(517, 47, LedKey.OEMMINUS),
            new LedKeyPoint(564, 47, LedKey.EQUALS),
            new LedKeyPoint(633, 47, LedKey.BACKSPACE),
            new LedKeyPoint(705, 47, LedKey.NUM_LOCK),
            new LedKeyPoint(752, 47, LedKey.DIVIDE),
            new LedKeyPoint(799, 47, LedKey.MULTIPLY),
            new LedKeyPoint(846, 47, LedKey.SUBTRACT),
            new LedKeyPoint(20, 94, LedKey.TAB),
            new LedKeyPoint(70, 94, LedKey.Q),
            new LedKeyPoint(117, 94, LedKey.W),
            new LedKeyPoint(167, 94, LedKey.E),
            new LedKeyPoint(211, 94, LedKey.R),
            new LedKeyPoint(258, 94, LedKey.T),
            new LedKeyPoint(305, 94, LedKey.Y),
            new LedKeyPoint(352, 94, LedKey.U),
            new LedKeyPoint(399, 94, LedKey.I),
            new LedKeyPoint(446, 94, LedKey.O),
            new LedKeyPoint(493, 94, LedKey.P),
            new LedKeyPoint(540, 94, LedKey.OPEN_BRACKET),
            new LedKeyPoint(587, 94, LedKey.CLOSE_BRACKET),
            new LedKeyPoint(705, 94, LedKey.NUMPAD7),
            new LedKeyPoint(752, 94, LedKey.NUMPAD8),
            new LedKeyPoint(799, 94, LedKey.NUMPAD9),
            new LedKeyPoint(846, 117, LedKey.ADD),
            new LedKeyPoint(25, 141, LedKey.EASY_SHIFT),
            new LedKeyPoint(38, 141, LedKey.A),
            new LedKeyPoint(85, 141, LedKey.S),
            new LedKeyPoint(132, 141, LedKey.D),
            new LedKeyPoint(179, 141, LedKey.F),
            new LedKeyPoint(226, 141, LedKey.G),
            new LedKeyPoint(273, 141, LedKey.H),
            new LedKeyPoint(320, 141, LedKey.J),
            new LedKeyPoint(367, 141, LedKey.K),
            new LedKeyPoint(414, 141, LedKey.L),
            new LedKeyPoint(461, 141, LedKey.SEMICOLON),
            new LedKeyPoint(508, 141, LedKey.APOSTROPHE),
            new LedKeyPoint(555, 141, LedKey.ISO_HASH),
            new LedKeyPoint(630, 164, LedKey.ENTER),
            new LedKeyPoint(705, 141, LedKey.NUMPAD4),
            new LedKeyPoint(752, 141, LedKey.NUMPAD5),
            new LedKeyPoint(799, 141, LedKey.NUMPAD6),
            new LedKeyPoint(10, 188, LedKey.LEFT_SHIFT),
            new LedKeyPoint(52, 188, LedKey.ISO_BACKSLASH),
            new LedKeyPoint(99, 188, LedKey.Z),
            new LedKeyPoint(146, 188, LedKey.X),
            new LedKeyPoint(193, 188, LedKey.C),
            new LedKeyPoint(240, 188, LedKey.V),
            new LedKeyPoint(287, 188, LedKey.B),
            new LedKeyPoint(334, 188, LedKey.N),
            new LedKeyPoint(381, 188, LedKey.M),
            new LedKeyPoint(428, 188, LedKey.OEMCOMMA),
            new LedKeyPoint(475, 188, LedKey.OEMPERIOD),
            new LedKeyPoint(522, 188, LedKey.FORWARD_SLASH),
            new LedKeyPoint(587, 188, LedKey.RIGHT_SHIFT),
            new LedKeyPoint(658, 188, LedKey.ARROW_UP),
            new LedKeyPoint(705, 188, LedKey.NUMPAD1),
            new LedKeyPoint(752, 188, LedKey.NUMPAD2),
            new LedKeyPoint(799, 188, LedKey.NUMPAD3),
            new LedKeyPoint(846, 211, LedKey.NUM_ENTER),
            new LedKeyPoint(50, 235, LedKey.LEFT_CONTROL),
            new LedKeyPoint(55, 235, LedKey.LEFT_WINDOWS),
            new LedKeyPoint(117, 235, LedKey.LEFT_ALT),
            new LedKeyPoint(340, 235, LedKey.SPACE),
            new LedKeyPoint(470, 235, LedKey.RIGHT_ALT),
            new LedKeyPoint(517, 235, LedKey.FN_Key),
            new LedKeyPoint(564, 235, LedKey.RIGHT_CONTROL),
            new LedKeyPoint(611, 235, LedKey.ARROW_LEFT),
            new LedKeyPoint(658, 235, LedKey.ARROW_DOWN),
            new LedKeyPoint(705, 235, LedKey.ARROW_RIGHT),
            new LedKeyPoint(752, 235, LedKey.NUMPAD0),
            new LedKeyPoint(799, 235, LedKey.DECIMAL),

        };

        /// <summary>
        /// Initializes the keyboard. Returns a keyboard object if initialized successfully or null otherwise
        /// </summary>
        public static QMKKeyboard Initialize()
        {
            var devices = DeviceList.Local.GetHidDevices(vendorID: 0x3434).FirstOrDefault(x => x.GetMaxInputReportLength() == x.GetMaxOutputReportLength() && x.GetMaxInputReportLength() >= RAW_EPSIZE);

            try
            {
                HidDevice ctrlDevice = devices;
                HidStream ctrlStream = null;

                if ((ctrlDevice?.TryOpen(out ctrlStream) ?? false))
                {
                    var kb = new QMKKeyboard(ctrlStream);

                    var desc = devices.GetReportDescriptor();

                    var inputRec = new HidDeviceInputReceiver(100, desc);
                    inputRec.Stopped += (s, e) =>
                    {
                        Console.WriteLine("Got stopped");
                        while (!devices.TryOpen(out ctrlStream))
                        {
                            Thread.Sleep(100);
                        }
                        inputRec.Start(ctrlStream);
                        Console.WriteLine("Reconnected");
                    };
                    inputRec.Received += kb.Receiver1Received;
                    inputRec.Start(ctrlStream);

                    return kb;
                }
                else
                {
                    ctrlStream?.Close();
                }
            }
            catch
            { }

            return null;
        }



        public bool Connect()
        {
            var ctrlDevice = DeviceList.Local.GetHidDevices(vendorID: 0x3434).FirstOrDefault(x => x.GetMaxInputReportLength() == x.GetMaxOutputReportLength() && x.GetMaxInputReportLength() >= RAW_EPSIZE);

            if (ctrlDevice is null)
                return false;
            if (connected)
                Disconnect();

            try
            {
                if ((ctrlDevice?.TryOpen(out _ctrlStream) ?? false))
                {
                    _source = new CancellationTokenSource();

                    updateOperationThread = new Thread(() => UpdateThread(_source.Token))
                    {
                        IsBackground = true
                    };
                    updateOperationThread.Start();

                    var desc = ctrlDevice.GetReportDescriptor();

                    var inputRec = new HidDeviceInputReceiver(100, desc);
                    inputRec.Stopped += (s, e) =>
                    {
                        Console.WriteLine("Got stopped");
                        while (!ctrlDevice.TryOpen(out _ctrlStream))
                        {
                            Thread.Sleep(100);
                        }
                        inputRec.Start(_ctrlStream);
                        Console.WriteLine("Reconnected");
                    };
                    inputRec.Received += Receiver1Received;
                    inputRec.Start(_ctrlStream);

                    return true;
                }
                else
                {
                    _ctrlStream?.Close();
                }
            }
            catch
            { }

            return false;
        }

        private void Receiver1Received(object sender, ByteEventArgs e)
        {
            var mode = (MainModes)e.Bytes[e.Offset + 1];
            var subMode = (OtherModes)e.Bytes[e.Offset + 2];
            if (mode == MainModes.OTHER_COMMAND && subMode == OtherModes.CustomKeyCode)
            {

                var data = e.Bytes[(e.Offset + 3)..(e.Offset + 12)];
                //{
                //    var index = data[0];
                //    var dataToSend = new byte[6];
                //    dataToSend[0] = reportId++;
                //    dataToSend[1] = (byte)Modes.PerKeyRGB;.
                //    dataToSend[2] = index;
                //    dataToSend[ 3] = (byte)random.Next(0, 255);
                //    dataToSend[ 4] = (byte)random.Next(0, 255);
                //    dataToSend[ 5] = (byte)random.Next(0, 255);

                //    //ds.Write(dataToSend);
                //}
                var columnRow = (data[3], data[4]);
                if (columnRow == (18, 0) /*MyKeycodes.MYCKC_RGB*/)
                    VolumeKnobTurnedReceived?.Invoke(this, new VolumeKnDirectionArgs(0, data[5] == 1));

                else if (RowColumnToLedKeyMapping.TryGetValue(columnRow, out var mapping))
                    KeyPressedReceived?.Invoke(this, new KeyPressedArgs(mapping, data[5] == 1));
                if (data[1] == 0)
                {
                    /*
                    Key    Pressedamount    Column  Row     Pressed     Timer and so on
                    c3 5c       00          12      00      01          00 b9 95
                    c3 5c       00          12      00      00          00 37 96
                     */
                    var kp = (HidKeyboardKeypadUsage)data[0];

                    //if(kp == HidKeyboardKeypadUsage.KC_UP && data[5] == 0)
                    //{
                    //    tmpindex++;
                    //}

                    //else if (kp == HidKeyboardKeypadUsage.KC_DOWN && data[5] == 0)
                    //{
                    //    tmpindex--;
                    //}
                    Console.WriteLine($"{kp} {(data[5] == 0 ? "Released" : "Pressed")} {data[2] >> 4} times");
                }
                else
                {
                    var keycode = (MyKeycodes)((data[1] << 8) | data[0]);
                    if (Enum.IsDefined<MyKeycodes>(keycode))
                        Console.WriteLine($"{keycode} {(data[5] == 0 ? "Released" : "Pressed")} {data[2] >> 4} times");
                    else
                        Console.WriteLine($"{keycode:x} {(data[5] == 0 ? "Released" : "Pressed")} {data[2] >> 4} times");
                }
            }
        }


        #region Public Methods
        /// <summary>
        /// Sets the whole keyboard to a color
        /// </summary>
        public void SetColor(Color clr)
        {
            foreach (LedKey key in (LedKey[])Enum.GetValues(typeof(LedKey)))
                SetKeyColor(key, clr);
        }

        /// <summary>
        /// Set the colors of all the keys in the dictionary
        /// </summary>
        public void SetColors(Dictionary<LedKey, Color> keyColors)
        {
            foreach (var key in keyColors)
                SetKeyColor(key.Key, key.Value);
        }

        /// <summary>
        /// Set the colors of all the keys in the dictionary
        /// </summary>
        public void SetColors(Dictionary<int, Color> keyColors)
        {
            foreach (var key in keyColors)
                SetKeyColor(key.Key, key.Value);
        }

        /// <summary>
        /// Sets a given key to a given color
        /// </summary>
        public void SetKeyColor(LedKey key, Color clr)
        {
            var indx = LedKeyIndicesMapping.IndexOf(key);
            if (indx != -1)
            {
                var rgbOff = indx * 3;
                _keyColors[rgbOff] = clr.R;
                _keyColors[rgbOff + 1] = clr.G;
                _keyColors[rgbOff + 2] = clr.B;
            }
            else
                ;
        }

        /// <summary>
        /// Sets a given key to a given color
        /// </summary>
        public void SetKeyColor(int key, Color clr)
        {
            var offset = (int)(byte)key;
            var rgbOff = offset * 3;
            _keyColors[rgbOff] = clr.R;
            _keyColors[rgbOff + 1] = clr.G;
            _keyColors[rgbOff + 2] = clr.B;
        }

        public byte[] GetLastSendColorsCopy()
        {
            return _keyColors.ToArray();
        }

        private class UpdateOperation : IDisposable
        {
            private readonly QMKKeyboard keyboard;
            private readonly byte[] brightnessAdjusted;
            private readonly SemaphoreSlim updateDone;
            private bool success;

            public UpdateOperation(QMKKeyboard keyboard, byte[] brightnessAdjusted)
            {
                this.keyboard = keyboard;
                this.brightnessAdjusted = brightnessAdjusted;
                this.updateDone = new SemaphoreSlim(0);
            }

            public void Dispose()
            {
                updateDone.Dispose();
            }

            public void DoUpdate()
            {
                success = keyboard.WriteColorBuffer(brightnessAdjusted);
                updateDone.Release();
            }

            public async Task<bool> WaitAsync()
            {
                await updateDone.WaitAsync();
                return success;
            }
        }

        private UpdateOperation EnqueueUpdate(byte[] brightnessAdjusted)
        {
            var updateOperation = new UpdateOperation(this, brightnessAdjusted);
            Interlocked.Exchange(ref this.updateOperation, updateOperation)?.Dispose();
            updateOperationResetEvent.Set();
            return updateOperation;
        }

        private void UpdateThread(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!updateOperationResetEvent.WaitOne())
                    continue;

                var updateItem = Interlocked.Exchange(ref updateOperation, null);

                updateItem?.DoUpdate();
                Thread.Sleep(16);
            }
        }

        /// <summary>
        /// Writes data to the keyboard
        /// </summary>
        public async Task<bool> Update()
        {
            var adjusted = CreateBrightnessAdjustedBuffer();
            var workItem = EnqueueUpdate(adjusted);
            return await workItem.WaitAsync().ContinueWith((passthrough) =>
            {
                workItem.Dispose();
                return passthrough.Result;
            });
        }

        /// <summary>
        /// Disconnects from the keyboard. Call this last
        /// </summary>
        public void Disconnect()
        {
            connected = false;

            _source?.Cancel();
            streamsToDispose.ForEach(x => { x.Dispose(); });
            _ctrlStream?.Dispose();

        }

        public IReadOnlyCollection<LedKeyPoint> GetLedKeyPoints() => LedKeyPoints;


        #endregion

        #region Private Hid Methods
        //byte tmpindex = 21;
        private byte[] CreateBrightnessAdjustedBuffer()
        {
            byte[] brightnessAdjustedBuffer;

            if (brightness == 69)
            {
                brightnessAdjustedBuffer = _keyColors.ToArray();
            }
            else
            {
                brightnessAdjustedBuffer = new byte[_keyColors.Length];
                for (int i = 0; i < brightnessAdjustedBuffer.Length; i++)
                {
                    brightnessAdjustedBuffer[i] = (byte)(_keyColors[i] / 69.0 * brightness);
                }
            }
            //for (int i = 0; i < brightnessAdjustedBuffer.Length / 3; i++)
            //{
            //    brightnessAdjustedBuffer[i * 3] = i == tmpindex ? brightnessAdjustedBuffer[i * 3] : (byte)0;
            //    brightnessAdjustedBuffer[i * 3 + 1] = i == tmpindex ? brightnessAdjustedBuffer[i * 3 + 1] : (byte)0;
            //    brightnessAdjustedBuffer[i * 3 + 2] = i == tmpindex ? brightnessAdjustedBuffer[i * 3 + 2] : (byte)0;
            //}

            //tmpindex++;
            //tmpindex %= 101;

            return brightnessAdjustedBuffer;
        }

        byte[] packet = new byte[RAW_EPSIZE + 1];
        int packageCount = 0;
        private bool connected;

        private bool WriteColorBuffer(byte[] brightnessAdjustedBuffer)
        {
            if (!connected)
            {
                connected = Connect();
                return false;
            }
            const byte keysPerTransimission = (RAW_EPSIZE - 2) / 3;
            const byte bytesPerTransimission = keysPerTransimission * 3;
            const byte keysFirstTransimission = (RAW_EPSIZE - 1) / 3;
            const byte bytesFirstTransimission = keysFirstTransimission * 3;
            const bool specialFirstPackage = keysPerTransimission != keysFirstTransimission;

            byte[] toSend = new byte[] { };

            ushort rgbheader = 0;
            var packageamount = (brightnessAdjustedBuffer.Length - bytesFirstTransimission) / bytesPerTransimission + 1 + Math.Min(1, (brightnessAdjustedBuffer.Length - bytesFirstTransimission) % bytesPerTransimission);
            for (int i = 0; i < packageamount; i++)
            {
                //packet[0] = ReportId
                var keysForThisMessage = Math.Min((brightnessAdjustedBuffer.Length / 3) - (keysFirstTransimission + (i - 1) * keysPerTransimission), keysPerTransimission);

                var mainMode = (ushort)MainModes.RGB_COMMAND;
                var count = (ushort)(i == 0 && specialFirstPackage ? keysFirstTransimission : keysForThisMessage); //Length in keys
                var rgbmode = (ushort)(i == 0 && specialFirstPackage ? RGBModes.IndexItereationRGBZero : RGBModes.IndexItereationRGB);
                var index = (ushort)(i * keysPerTransimission + 1);

                rgbheader = (ushort)(((mainMode & 0b11) << 14) | ((count & 0b11111) << 9) | ((rgbmode & 0b11) << 7) | (index & 0b1111111));
                packet[0] = ReportId;
                packet[1] = (byte)(rgbheader >> 8);
                packet[2] = (byte)(rgbheader & 0xFF);

                if (i == 0 && specialFirstPackage)
                    Array.Copy(brightnessAdjustedBuffer, 0, packet, 2, bytesFirstTransimission);
                else
                    Array.Copy(brightnessAdjustedBuffer, Math.Max(i - 1, 0) * bytesPerTransimission + bytesFirstTransimission, packet, 3, keysForThisMessage * 3);

                toSend = toSend.Concat(packet).ToArray();
            }

            try
            {
                packageCount += packageamount;
                if (packageCount % (packageamount * 200) == 0)
                {
                    Debug.WriteLine(packageCount);

                }
                _ctrlStream.Write(toSend);//, 0, /*RAW_EPSIZE + 1*/toSend.Length);
            }
            catch (Exception e)
            {
                //ArrayPool<byte>.Shared.Return(packet);
                Disconnect();
                return false;
            }
            return true;

        }


        #endregion

        #region IDisposable Support
        /// <summary>
        /// Disconnects the keyboard when disposing
        /// </summary>
        public void Dispose() => Disconnect();
        public bool SetColors(ReadOnlySpan<byte> keyColors) => throw new NotImplementedException();
        #endregion
    }
}
