using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.VirtualHID
{
    internal class ScanCodeMapping
    {
        internal static readonly IReadOnlyDictionary<int, ushort> USBToWin = new Dictionary<int, ushort>() {
            { 0x000000, 0x0000 },  // Invalid
            { 0x010082, 0x0000 },  // SystemSleep
            { 0x010083, 0x0000 },  // SystemWakeUp
            { 0x070000, 0x0000 },  // Reserved
            { 0x070001, 0x0000 },  // ErrorRollOver
            { 0x070002, 0x0000 },  // POSTFail
            { 0x070003, 0x0000 },  // ErrorUndefined
            { 0x070004, 0x001e },  // aA
            { 0x070005, 0x0030 },  // bB
            { 0x070006, 0x002e },  // cC
            { 0x070007, 0x0020 },  // dD
            { 0x070008, 0x0012 },  // eE
            { 0x070009, 0x0021 },  // fF
            { 0x07000a, 0x0022 },  // gG
            { 0x07000b, 0x0023 },  // hH
            { 0x07000c, 0x0017 },  // iI
            { 0x07000d, 0x0024 },  // jJ
            { 0x07000e, 0x0025 },  // kK
            { 0x07000f, 0x0026 },  // lL
            { 0x070010, 0x0032 },  // mM
            { 0x070011, 0x0031 },  // nN
            { 0x070012, 0x0018 },  // oO
            { 0x070013, 0x0019 },  // pP
            { 0x070014, 0x0010 },  // qQ
            { 0x070015, 0x0013 },  // rR
            { 0x070016, 0x001f },  // sS
            { 0x070017, 0x0014 },  // tT
            { 0x070018, 0x0016 },  // uU
            { 0x070019, 0x002f },  // vV
            { 0x07001a, 0x0011 },  // wW
            { 0x07001b, 0x002d },  // xX
            { 0x07001c, 0x0015 },  // yY
            { 0x07001d, 0x002c },  // zZ
            { 0x07001e, 0x0002 },  // 1!
            { 0x07001f, 0x0003 },  // 2@
            { 0x070020, 0x0004 },  // 3#
            { 0x070021, 0x0005 },  // 4$
            { 0x070022, 0x0006 },  // 5%
            { 0x070023, 0x0007 },  // 6^
            { 0x070024, 0x0008 },  // 7&
            { 0x070025, 0x0009 },  // 8*
            { 0x070026, 0x000a },  // 9(
            { 0x070027, 0x000b },  // 0)
            { 0x070028, 0x001c },  // Return
            { 0x070029, 0x0001 },  // Escape
            { 0x07002a, 0x000e },  // Backspace
            { 0x07002b, 0x000f },  // Tab
            { 0x07002c, 0x0039 },  // Spacebar
            { 0x07002d, 0x000c },  // -_
            { 0x07002e, 0x000d },  // =+
            { 0x07002f, 0x001a },  // [{
            { 0x070030, 0x001b },  // }]
            //{ 0x070031, 0x002b },  // \| (US keyboard only)
            { 0x070032, 0x002b },  // #~ (Non-US)
            { 0x070033, 0x0027 },  // ;:
            { 0x070034, 0x0028 },  // '"
            { 0x070035, 0x0029 },  // `~
            { 0x070036, 0x0033 },  // ,<
            { 0x070037, 0x0034 },  // .>
            { 0x070038, 0x0035 },  // /?
            { 0x070039, 0x003a },  // CapsLock
            { 0x07003a, 0x003b },  // F1
            { 0x07003b, 0x003c },  // F2
            { 0x07003c, 0x003d },  // F3
            { 0x07003d, 0x003e },  // F4
            { 0x07003e, 0x003f },  // F5
            { 0x07003f, 0x0040 },  // F6
            { 0x070040, 0x0041 },  // F7
            { 0x070041, 0x0042 },  // F8
            { 0x070042, 0x0043 },  // F9
            { 0x070043, 0x0044 },  // F10
            { 0x070044, 0x0057 },  // F11
            { 0x070045, 0x0058 },  // F12
            { 0x070046, 0xe037 },  // PrintScreen
            { 0x070047, 0x0046 },  // ScrollLock
            { 0x070048, 0x0000 },  // Pause
            { 0x070049, 0xe052 },  // Insert
            { 0x07004a, 0xe047 },  // Home
            { 0x07004b, 0xe049 },  // PageUp
            { 0x07004c, 0xe053 },  // Delete (Forward Delete)
            { 0x07004d, 0xe04f },  // End
            { 0x07004e, 0xe051 },  // PageDown
            { 0x07004f, 0xe04d },  // RightArrow
            { 0x070050, 0xe04b },  // LeftArrow
            { 0x070051, 0xe050 },  // DownArrow
            { 0x070052, 0xe048 },  // UpArrow
            { 0x070053, 0x0045 },  // Keypad_NumLock Clear
            { 0x070054, 0xe035 },  // Keypad_/
            { 0x070055, 0x0037 },  // Keypad_*
            { 0x070056, 0x004a },  // Keypad_-
            { 0x070057, 0x004e },  // Keypad_+
            { 0x070058, 0xe01c },  // Keypad_Enter
            { 0x070059, 0x004f },  // Keypad_1 End
            { 0x07005a, 0x0050 },  // Keypad_2 DownArrow
            { 0x07005b, 0x0051 },  // Keypad_3 PageDown
            { 0x07005c, 0x004b },  // Keypad_4 LeftArrow
            { 0x07005d, 0x004c },  // Keypad_5
            { 0x07005e, 0x004d },  // Keypad_6 RightArrow
            { 0x07005f, 0x0047 },  // Keypad_7 Home
            { 0x070060, 0x0048 },  // Keypad_8 UpArrow
            { 0x070061, 0x0049 },  // Keypad_9 PageUp
            { 0x070062, 0x0052 },  // Keypad_0 Insert
            { 0x070063, 0x0053 },  // Keypad_. Delete
            { 0x070064, 0x0056 },  // Non-US \|
            { 0x070065, 0xe05d },  // AppMenu (next to RWin key)
            { 0x070066, 0x0000 },  // Power
            { 0x070067, 0x0000 },  // Keypad_=
            { 0x070068, 0x005b },  // F13
            { 0x070069, 0x005c },  // F14
            { 0x07006a, 0x005d },  // F15
            { 0x07006b, 0x0063 },  // F16
            { 0x07006c, 0x0064 },  // F17
            { 0x07006d, 0x0065 },  // F18
            { 0x07006e, 0x0066 },  // F19
            { 0x07006f, 0x0067 },  // F20
            { 0x070070, 0x0068 },  // F21
            { 0x070071, 0x0069 },  // F22
            { 0x070072, 0x006a },  // F23
            { 0x070073, 0x006b },  // F24
            { 0x070074, 0x0000 },  // Execute
            { 0x070075, 0xe03b },  // Help
            { 0x070076, 0x0000 },  // Menu
            { 0x070077, 0x0000 },  // Select
            { 0x070078, 0x0000 },  // Stop
            { 0x070079, 0x0000 },  // Again (Redo)
            { 0x07007a, 0xe008 },  // Undo
            { 0x07007b, 0xe017 },  // Cut
            { 0x07007c, 0xe018 },  // Copy
            { 0x07007d, 0xe00a },  // Paste
            { 0x07007e, 0x0000 },  // Find
            { 0x07007f, 0xe020 },  // Mute
            { 0x070080, 0xe030 },  // VolumeUp
            { 0x070081, 0xe02e },  // VolumeDown
            { 0x070082, 0x0000 },  // LockingCapsLock
            { 0x070083, 0x0000 },  // LockingNumLock
            { 0x070084, 0x0000 },  // LockingScrollLock
            { 0x070085, 0x0000 },  // Keypad_Comma
            { 0x070087, 0x0000 },  // International1
            { 0x070088, 0x0000 },  // International2
            { 0x070089, 0x007d },  // International3
            { 0x07008a, 0x0000 },  // International4
            { 0x07008b, 0x0000 },  // International5
            { 0x07008c, 0x0000 },  // International6
            { 0x07008d, 0x0000 },  // International7
            { 0x07008e, 0x0000 },  // International8
            { 0x07008f, 0x0000 },  // International9
            { 0x070090, 0x0000 },  // LANG1
            { 0x070091, 0x0000 },  // LANG2
            { 0x070092, 0x0000 },  // LANG3
            { 0x070093, 0x0000 },  // LANG4
            { 0x070094, 0x0000 },  // LANG5
            { 0x070095, 0x0000 },  // LANG6
            { 0x070096, 0x0000 },  // LANG7
            { 0x070097, 0x0000 },  // LANG8
            { 0x070098, 0x0000 },  // LANG9
            { 0x070099, 0x0000 },  // AlternateErase
            { 0x07009a, 0x0000 },  // SysReq/Attention
            { 0x07009b, 0x0000 },  // Cancel
            { 0x07009c, 0x0000 },  // Clear
            { 0x07009d, 0x0000 },  // Prior
            { 0x07009e, 0x0000 },  // Return
            { 0x07009f, 0x0000 },  // Separator
            { 0x0700a0, 0x0000 },  // Out
            { 0x0700a1, 0x0000 },  // Oper
            { 0x0700a2, 0x0000 },  // Clear/Again
            { 0x0700a3, 0x0000 },  // CrSel/Props
            { 0x0700a4, 0x0000 },  // ExSel
            { 0x0700b6, 0x0000 },  // Keypad_(
            { 0x0700b7, 0x0000 },  // Keypad_)
            { 0x0700d7, 0x0000 },  // Keypad_+/-
            { 0x0700dc, 0x0000 },  // Keypad_Decimal
            { 0x0700e0, 0x001d },  // LeftControl
            { 0x0700e1, 0x002a },  // LeftShift
            { 0x0700e2, 0x0038 },  // LeftAlt/Option
            { 0x0700e3, 0xe05b },  // LeftGUI/Super/Win/Cmd
            { 0x0700e4, 0xe01d },  // RightControl
            { 0x0700e5, 0x0036 },  // RightShift
            { 0x0700e6, 0xe038 },  // RightAlt/Option
            { 0x0700e7, 0xe05c },  // RightGUI/Super/Win/Cmd
            { 0x0c00b5, 0xe019 },  // ScanNextTrack
            { 0x0c00b6, 0xe010 },  // ScanPreviousTrack
            { 0x0c00b7, 0xe024 },  // Stop
            { 0x0c00b8, 0xe02c },  // Eject
            { 0x0c00cd, 0xe022 },  // Play/Pause
            { 0x0c018a, 0xe01e },  // AL_EmailReader
            { 0x0c0192, 0x0000 },  // AL_Calculator
            { 0x0c0194, 0x0000 },  // AL_LocalMachineBrowser
            { 0x0c01a7, 0x0000 },  // AL_Documents
            { 0x0c01b4, 0x0000 },  // AL_FileBrowser (Explorer)
            { 0x0c0221, 0xe065 },  // AC_Search
            { 0x0c0223, 0xe032 },  // AC_Home
            { 0x0c0224, 0xe06a },  // AC_Back
            { 0x0c0225, 0xe069 },  // AC_Forward
            { 0x0c0226, 0xe068 },  // AC_Stop
            { 0x0c0227, 0xe067 },  // AC_Refresh (Reload)
            { 0x0c022a, 0xe066 },  // AC_Bookmarks (Favorites)
            { 0x0c0289, 0x0000 },  // AC_Reply
            { 0x0c028b, 0x0000 },  // AC_ForwardMsg (MailForwar
        };
        internal static readonly IReadOnlyDictionary<ushort, int> WinToUSB;

        static ScanCodeMapping()
        {
            WinToUSB = USBToWin.Where(x=>x.Value != 0).ToDictionary(x => x.Value, x => x.Key);
        }

    }
}
