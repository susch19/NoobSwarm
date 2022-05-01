
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.GenericKeyboard
{
    enum HidKeyboardKeypadUsage : byte
    {
        KC_NO = 0x00,
        KC_ROLL_OVER,
        KC_POST_FAIL,
        KC_UNDEFINED,
        KC_A,
        KC_B,
        KC_C,
        KC_D,
        KC_E,
        KC_F,
        KC_G,
        KC_H,
        KC_I,
        KC_J,
        KC_K,
        KC_L,
        KC_M,  // 0x10
        KC_N,
        KC_O,
        KC_P,
        KC_Q,
        KC_R,
        KC_S,
        KC_T,
        KC_U,
        KC_V,
        KC_W,
        KC_X,
        KC_Y,
        KC_Z,
        KC_1,
        KC_2,
        KC_3,  // 0x20
        KC_4,
        KC_5,
        KC_6,
        KC_7,
        KC_8,
        KC_9,
        KC_0,
        KC_ENTER,
        KC_ESCAPE,
        KC_BACKSPACE,
        KC_TAB,
        KC_SPACE,
        KC_MINUS,
        KC_EQUAL,
        KC_LEFT_BRACKET,
        KC_RIGHT_BRACKET,  // 0x30
        KC_BACKSLASH,
        KC_NONUS_HASH,
        KC_SEMICOLON,
        KC_QUOTE,
        KC_GRAVE,
        KC_COMMA,
        KC_DOT,
        KC_SLASH,
        KC_CAPS_LOCK,
        KC_F1,
        KC_F2,
        KC_F3,
        KC_F4,
        KC_F5,
        KC_F6,
        KC_F7,  // 0x40
        KC_F8,
        KC_F9,
        KC_F10,
        KC_F11,
        KC_F12,
        KC_PRINT_SCREEN,
        KC_SCROLL_LOCK,
        KC_PAUSE,
        KC_INSERT,
        KC_HOME,
        KC_PAGE_UP,
        KC_DELETE,
        KC_END,
        KC_PAGE_DOWN,
        KC_RIGHT,
        KC_LEFT,  // 0x50
        KC_DOWN,
        KC_UP,
        KC_NUM_LOCK,
        KC_KP_SLASH,
        KC_KP_ASTERISK,
        KC_KP_MINUS,
        KC_KP_PLUS,
        KC_KP_ENTER,
        KC_KP_1,
        KC_KP_2,
        KC_KP_3,
        KC_KP_4,
        KC_KP_5,
        KC_KP_6,
        KC_KP_7,
        KC_KP_8,  // 0x60
        KC_KP_9,
        KC_KP_0,
        KC_KP_DOT,
        KC_NONUS_BACKSLASH,
        KC_APPLICATION,
        KC_KB_POWER,
        KC_KP_EQUAL,
        KC_F13,
        KC_F14,
        KC_F15,
        KC_F16,
        KC_F17,
        KC_F18,
        KC_F19,
        KC_F20,
        KC_F21,  // 0x70
        KC_F22,
        KC_F23,
        KC_F24,
        KC_EXECUTE,
        KC_HELP,
        KC_MENU,
        KC_SELECT,
        KC_STOP,
        KC_AGAIN,
        KC_UNDO,
        KC_CUT,
        KC_COPY,
        KC_PASTE,
        KC_FIND,
        KC_KB_MUTE,
        KC_KB_VOLUME_UP,  // 0x80
        KC_KB_VOLUME_DOWN,
        KC_LOCKING_CAPS_LOCK,
        KC_LOCKING_NUM_LOCK,
        KC_LOCKING_SCROLL_LOCK,
        KC_KP_COMMA,
        KC_KP_EQUAL_AS400,
        KC_INTERNATIONAL_1,
        KC_INTERNATIONAL_2,
        KC_INTERNATIONAL_3,
        KC_INTERNATIONAL_4,
        KC_INTERNATIONAL_5,
        KC_INTERNATIONAL_6,
        KC_INTERNATIONAL_7,
        KC_INTERNATIONAL_8,
        KC_INTERNATIONAL_9,
        KC_LANGUAGE_1,  // 0x90
        KC_LANGUAGE_2,
        KC_LANGUAGE_3,
        KC_LANGUAGE_4,
        KC_LANGUAGE_5,
        KC_LANGUAGE_6,
        KC_LANGUAGE_7,
        KC_LANGUAGE_8,
        KC_LANGUAGE_9,
        KC_ALTERNATE_ERASE,
        KC_SYSTEM_REQUEST,
        KC_CANCEL,
        KC_CLEAR,
        KC_PRIOR,
        KC_RETURN,
        KC_SEPARATOR,
        KC_OUT,  // 0xA0
        KC_OPER,
        KC_CLEAR_AGAIN,
        KC_CRSEL,
        KC_EXSEL,


        /* Modifiers */
        KC_LEFT_CTRL = 0xE0,
        KC_LEFT_SHIFT,
        KC_LEFT_ALT,
        KC_LEFT_GUI,
        KC_RIGHT_CTRL,
        KC_RIGHT_SHIFT,
        KC_RIGHT_ALT,
        KC_RIGHT_GUI

        // **********************************************
        // * 0xF0-0xFF are unallocated in the HID spec. *
        // * QMK uses these for Mouse Keys - see below. *
        // **********************************************
    };

    /* Media and Function keys */
    enum internal_special_keycodes
    {
        /* Generic Desktop Page (0x01) */
        KC_SYSTEM_POWER = 0xA5,
        KC_SYSTEM_SLEEP,
        KC_SYSTEM_WAKE,

        /* Consumer Page (0x0C) */
        KC_AUDIO_MUTE,
        KC_AUDIO_VOL_UP,
        KC_AUDIO_VOL_DOWN,
        KC_MEDIA_NEXT_TRACK,
        KC_MEDIA_PREV_TRACK,
        KC_MEDIA_STOP,
        KC_MEDIA_PLAY_PAUSE,
        KC_MEDIA_SELECT,
        KC_MEDIA_EJECT,  // 0xB0
        KC_MAIL,
        KC_CALCULATOR,
        KC_MY_COMPUTER,
        KC_WWW_SEARCH,
        KC_WWW_HOME,
        KC_WWW_BACK,
        KC_WWW_FORWARD,
        KC_WWW_STOP,
        KC_WWW_REFRESH,
        KC_WWW_FAVORITES,
        KC_MEDIA_FAST_FORWARD,
        KC_MEDIA_REWIND,
        KC_BRIGHTNESS_UP,
        KC_BRIGHTNESS_DOWN,

        /* Fn keys */
        KC_FN0 = 0xC0,
        KC_FN1,
        KC_FN2,
        KC_FN3,
        KC_FN4,
        KC_FN5,
        KC_FN6,
        KC_FN7,
        KC_FN8,
        KC_FN9,
        KC_FN10,
        KC_FN11,
        KC_FN12,
        KC_FN13,
        KC_FN14,
        KC_FN15,
        KC_FN16,  // 0xD0
        KC_FN17,
        KC_FN18,
        KC_FN19,
        KC_FN20,
        KC_FN21,
        KC_FN22,
        KC_FN23,
        KC_FN24,
        KC_FN25,
        KC_FN26,
        KC_FN27,
        KC_FN28,
        KC_FN29,
        KC_FN30,
        KC_FN31
    };

    enum mouse_keys
    {
        /* Mouse Buttons */
        KC_MS_UP = 0xED,
        KC_MS_DOWN,
        KC_MS_LEFT,
        KC_MS_RIGHT,  // 0xF0
        KC_MS_BTN1,
        KC_MS_BTN2,
        KC_MS_BTN3,
        KC_MS_BTN4,
        KC_MS_BTN5,
        KC_MS_BTN6,
        KC_MS_BTN7,
        KC_MS_BTN8,

        /* Mouse Wheel */
        KC_MS_WH_UP,
        KC_MS_WH_DOWN,
        KC_MS_WH_LEFT,
        KC_MS_WH_RIGHT,

        /* Acceleration */
        KC_MS_ACCEL0,
        KC_MS_ACCEL1,
        KC_MS_ACCEL2  // 0xFF
    };

    enum quantum_keycodes
    {
        // Ranges used in shortcuts - not to be used directly
        QK_BASIC = 0x0000,
        QK_BASIC_MAX = 0x00FF,
        QK_MODS = 0x0100,
        QK_LCTL = 0x0100,
        QK_LSFT = 0x0200,
        QK_LALT = 0x0400,
        QK_LGUI = 0x0800,
        QK_RMODS_MIN = 0x1000,
        QK_RCTL = 0x1100,
        QK_RSFT = 0x1200,
        QK_RALT = 0x1400,
        QK_RGUI = 0x1800,
        QK_MODS_MAX = 0x1FFF,
        QK_FUNCTION = 0x2000,
        QK_FUNCTION_MAX = 0x2FFF,
        QK_MACRO = 0x3000,
        QK_MACRO_MAX = 0x3FFF,
        QK_LAYER_TAP = 0x4000,
        QK_LAYER_TAP_MAX = 0x4FFF,
        QK_TO = 0x5000,
        QK_TO_MAX = 0x50FF,
        QK_MOMENTARY = 0x5100,
        QK_MOMENTARY_MAX = 0x51FF,
        QK_DEF_LAYER = 0x5200,
        QK_DEF_LAYER_MAX = 0x52FF,
        QK_TOGGLE_LAYER = 0x5300,
        QK_TOGGLE_LAYER_MAX = 0x53FF,
        QK_ONE_SHOT_LAYER = 0x5400,
        QK_ONE_SHOT_LAYER_MAX = 0x54FF,
        QK_ONE_SHOT_MOD = 0x5500,
        QK_ONE_SHOT_MOD_MAX = 0x55FF,
        QK_SWAP_HANDS = 0x5600,
        QK_SWAP_HANDS_MAX = 0x56FF,
        QK_TAP_DANCE = 0x5700,
        QK_TAP_DANCE_MAX = 0x57FF,
        QK_LAYER_TAP_TOGGLE = 0x5800,
        QK_LAYER_TAP_TOGGLE_MAX = 0x58FF,
        QK_LAYER_MOD = 0x5900,
        QK_LAYER_MOD_MAX = 0x59FF,
        QK_STENO = 0x5A00,
        QK_STENO_BOLT = 0x5A30,
        QK_STENO_GEMINI = 0x5A31,
        QK_STENO_COMB = 0x5A32,
        QK_STENO_COMB_MAX = 0x5A3C,
        QK_STENO_MAX = 0x5A3F,
        // 0x5C00 - 0x5FFF are reserved, see below
        QK_MOD_TAP = 0x6000,
        QK_MOD_TAP_MAX = 0x7FFF,
        QK_UNICODE = 0x8000,
        QK_UNICODE_MAX = 0xFFFF,
        QK_UNICODEMAP = 0x8000,
        QK_UNICODEMAP_MAX = 0xBFFF,
        QK_UNICODEMAP_PAIR = 0xC000,
        QK_UNICODEMAP_PAIR_MAX = 0xFFFF,

        // Loose keycodes - to be used directly
        RESET = 0x5C00,
        DEBUG,  // 5C01

        // Magic
        MAGIC_SWAP_CONTROL_CAPSLOCK,       // 5C02
        MAGIC_CAPSLOCK_TO_CONTROL,         // 5C03
        MAGIC_SWAP_LALT_LGUI,              // 5C04
        MAGIC_SWAP_RALT_RGUI,              // 5C05
        MAGIC_NO_GUI,                      // 5C06
        MAGIC_SWAP_GRAVE_ESC,              // 5C07
        MAGIC_SWAP_BACKSLASH_BACKSPACE,    // 5C08
        MAGIC_HOST_NKRO,                   // 5C09
        MAGIC_SWAP_ALT_GUI,                // 5C0A
        MAGIC_UNSWAP_CONTROL_CAPSLOCK,     // 5C0B
        MAGIC_UNCAPSLOCK_TO_CONTROL,       // 5C0C
        MAGIC_UNSWAP_LALT_LGUI,            // 5C0D
        MAGIC_UNSWAP_RALT_RGUI,            // 5C0E
        MAGIC_UNNO_GUI,                    // 5C0F
        MAGIC_UNSWAP_GRAVE_ESC,            // 5C10
        MAGIC_UNSWAP_BACKSLASH_BACKSPACE,  // 5C11
        MAGIC_UNHOST_NKRO,                 // 5C12
        MAGIC_UNSWAP_ALT_GUI,              // 5C13
        MAGIC_TOGGLE_NKRO,                 // 5C14
        MAGIC_TOGGLE_ALT_GUI,              // 5C15

        // Grave Escape
        GRAVE_ESC,  // 5C16

        // Auto Shift
        KC_ASUP,   // 5C17
        KC_ASDN,   // 5C18
        KC_ASRP,   // 5C19
        KC_ASTG,   // 5C1A
        KC_ASON,   // 5C1B
        KC_ASOFF,  // 5C1C

        // Audio
        AU_ON,   // 5C1D
        AU_OFF,  // 5C1E
        AU_TOG,  // 5C1F

        // Audio Clicky
        CLICKY_TOGGLE,   // 5C20
        CLICKY_ENABLE,   // 5C21
        CLICKY_DISABLE,  // 5C22
        CLICKY_UP,       // 5C23
        CLICKY_DOWN,     // 5C24
        CLICKY_RESET,    // 5C25

        // Music mode
        MU_ON,   // 5C26
        MU_OFF,  // 5C27
        MU_TOG,  // 5C28
        MU_MOD,  // 5C29
        MUV_IN,  // 5C2A
        MUV_DE,  // 5C2B

        // MIDI
        MI_ON,   // 5C2C
        MI_OFF,  // 5C2D
        MI_TOG,  // 5C2E

        MI_C,   // 5C2F
        MI_Cs,  // 5C30
        MI_Db = MI_Cs,
        MI_D,   // 5C31
        MI_Ds,  // 5C32
        MI_Eb = MI_Ds,
        MI_E,   // 5C33
        MI_F,   // 5C34
        MI_Fs,  // 5C35
        MI_Gb = MI_Fs,
        MI_G,   // 5C36
        MI_Gs,  // 5C37
        MI_Ab = MI_Gs,
        MI_A,   // 5C38
        MI_As,  // 5C39
        MI_Bb = MI_As,
        MI_B,  // 5C3A

        MI_C_1,   // 5C3B
        MI_Cs_1,  // 5C3C
        MI_Db_1 = MI_Cs_1,
        MI_D_1,   // 5C3D
        MI_Ds_1,  // 5C3E
        MI_Eb_1 = MI_Ds_1,
        MI_E_1,   // 5C3F
        MI_F_1,   // 5C40
        MI_Fs_1,  // 5C41
        MI_Gb_1 = MI_Fs_1,
        MI_G_1,   // 5C42
        MI_Gs_1,  // 5C43
        MI_Ab_1 = MI_Gs_1,
        MI_A_1,   // 5C44
        MI_As_1,  // 5C45
        MI_Bb_1 = MI_As_1,
        MI_B_1,  // 5C46

        MI_C_2,   // 5C47
        MI_Cs_2,  // 5C48
        MI_Db_2 = MI_Cs_2,
        MI_D_2,   // 5C49
        MI_Ds_2,  // 5C4A
        MI_Eb_2 = MI_Ds_2,
        MI_E_2,   // 5C4B
        MI_F_2,   // 5C4C
        MI_Fs_2,  // 5C4D
        MI_Gb_2 = MI_Fs_2,
        MI_G_2,   // 5C4E
        MI_Gs_2,  // 5C4F
        MI_Ab_2 = MI_Gs_2,
        MI_A_2,   // 5C50
        MI_As_2,  // 5C51
        MI_Bb_2 = MI_As_2,
        MI_B_2,  // 5C52

        MI_C_3,   // 5C53
        MI_Cs_3,  // 5C54
        MI_Db_3 = MI_Cs_3,
        MI_D_3,   // 5C55
        MI_Ds_3,  // 5C56
        MI_Eb_3 = MI_Ds_3,
        MI_E_3,   // 5C57
        MI_F_3,   // 5C58
        MI_Fs_3,  // 5C59
        MI_Gb_3 = MI_Fs_3,
        MI_G_3,   // 5C5A
        MI_Gs_3,  // 5C5B
        MI_Ab_3 = MI_Gs_3,
        MI_A_3,   // 5C5C
        MI_As_3,  // 5C5D
        MI_Bb_3 = MI_As_3,
        MI_B_3,  // 5C5E

        MI_C_4,   // 5C5F
        MI_Cs_4,  // 5C60
        MI_Db_4 = MI_Cs_4,
        MI_D_4,   // 5C61
        MI_Ds_4,  // 5C62
        MI_Eb_4 = MI_Ds_4,
        MI_E_4,   // 5C63
        MI_F_4,   // 5C64
        MI_Fs_4,  // 5C65
        MI_Gb_4 = MI_Fs_4,
        MI_G_4,   // 5C66
        MI_Gs_4,  // 5C67
        MI_Ab_4 = MI_Gs_4,
        MI_A_4,   // 5C68
        MI_As_4,  // 5C69
        MI_Bb_4 = MI_As_4,
        MI_B_4,  // 5C6A

        MI_C_5,   // 5C6B
        MI_Cs_5,  // 5C6C
        MI_Db_5 = MI_Cs_5,
        MI_D_5,   // 5C6D
        MI_Ds_5,  // 5C6E
        MI_Eb_5 = MI_Ds_5,
        MI_E_5,   // 5C6F
        MI_F_5,   // 5C70
        MI_Fs_5,  // 5C71
        MI_Gb_5 = MI_Fs_5,
        MI_G_5,   // 5C72
        MI_Gs_5,  // 5C73
        MI_Ab_5 = MI_Gs_5,
        MI_A_5,   // 5C74
        MI_As_5,  // 5C75
        MI_Bb_5 = MI_As_5,
        MI_B_5,  // 5C76

        MI_OCT_N2,  // 5C77
        MI_OCT_N1,  // 5C78
        MI_OCT_0,   // 5C79
        MI_OCT_1,   // 5C7A
        MI_OCT_2,   // 5C7B
        MI_OCT_3,   // 5C7C
        MI_OCT_4,   // 5C7D
        MI_OCT_5,   // 5C7E
        MI_OCT_6,   // 5C7F
        MI_OCT_7,   // 5C80
        MI_OCTD,    // 5C81
        MI_OCTU,    // 5C82

        MI_TRNS_N6,  // 5C83
        MI_TRNS_N5,  // 5C84
        MI_TRNS_N4,  // 5C85
        MI_TRNS_N3,  // 5C86
        MI_TRNS_N2,  // 5C87
        MI_TRNS_N1,  // 5C88
        MI_TRNS_0,   // 5C89
        MI_TRNS_1,   // 5C8A
        MI_TRNS_2,   // 5C8B
        MI_TRNS_3,   // 5C8C
        MI_TRNS_4,   // 5C8D
        MI_TRNS_5,   // 5C8E
        MI_TRNS_6,   // 5C8F
        MI_TRNSD,    // 5C90
        MI_TRNSU,    // 5C91

        MI_VEL_0,  // 5C92

        MI_VEL_1,  // 5C93
        MI_VEL_2,   // 5C94
        MI_VEL_3,   // 5C95
        MI_VEL_4,   // 5C96
        MI_VEL_5,   // 5C97
        MI_VEL_6,   // 5C98
        MI_VEL_7,   // 5C99
        MI_VEL_8,   // 5C9A
        MI_VEL_9,   // 5C9B
        MI_VEL_10,  // 5C9C
        MI_VELD,    // 5C9D
        MI_VELU,    // 5C9E

        MI_CH1,   // 5C9F
        MI_CH2,   // 5CA0
        MI_CH3,   // 5CA1
        MI_CH4,   // 5CA2
        MI_CH5,   // 5CA3
        MI_CH6,   // 5CA4
        MI_CH7,   // 5CA5
        MI_CH8,   // 5CA6
        MI_CH9,   // 5CA7
        MI_CH10,  // 5CA8
        MI_CH11,  // 5CA9
        MI_CH12,  // 5CAA
        MI_CH13,  // 5CAB
        MI_CH14,  // 5CAC
        MI_CH15,  // 5CAD
        MI_CH16,  // 5CAE
        MI_CHD,   // 5CAF
        MI_CHU,   // 5CB0

        MI_ALLOFF,  // 5CB1

        MI_SUS,   // 5CB2
        MI_PORT,  // 5CB3
        MI_SOST,  // 5CB4
        MI_SOFT,  // 5CB5
        MI_LEG,   // 5CB6

        MI_MOD,    // 5CB7
        MI_MODSD,  // 5CB8
        MI_MODSU,  // 5CB9

        MI_BENDD,  // 5CBA
        MI_BENDU,  // 5CBB

        // Backlight
        BL_ON,    // 5CBC
        BL_OFF,   // 5CBD
        BL_DEC,   // 5CBE
        BL_INC,   // 5CBF
        BL_TOGG,  // 5CC0
        BL_STEP,  // 5CC1
        BL_BRTG,  // 5CC2

        // RGB underglow/matrix
        RGB_TOG,            // 5CC3
        RGB_MODE_FORWARD,   // 5CC4
        RGB_MODE_REVERSE,   // 5CC5
        RGB_HUI,            // 5CC6
        RGB_HUD,            // 5CC7
        RGB_SAI,            // 5CC8
        RGB_SAD,            // 5CC9
        RGB_VAI,            // 5CCA
        RGB_VAD,            // 5CCB
        RGB_SPI,            // 5CCC
        RGB_SPD,            // 5CCD
        RGB_MODE_PLAIN,     // 5CCE
        RGB_MODE_BREATHE,   // 5CCF
        RGB_MODE_RAINBOW,   // 5CD0
        RGB_MODE_SWIRL,     // 5CD1
        RGB_MODE_SNAKE,     // 5CD2
        RGB_MODE_KNIGHT,    // 5CD3
        RGB_MODE_XMAS,      // 5CD4
        RGB_MODE_GRADIENT,  // 5CD5
        RGB_MODE_RGBTEST,   // 5CD6

        // Velocikey
        VLK_TOG,  // 5CD7

        // Space Cadet
        KC_LSPO,    // 5CD8
        KC_RSPC,    // 5CD9
        KC_SFTENT,  // 5CDA

        // Thermal Printer
        PRINT_ON,   // 5CDB
        PRINT_OFF,  // 5CDC

        // Bluetooth: output selection
        OUT_AUTO,  // 5CDD
        OUT_USB,   // 5CDE

        // Clear EEPROM
        EEPROM_RESET,  // 5CDF

        // Unicode
        UNICODE_MODE_FORWARD,  // 5CE0
        UNICODE_MODE_REVERSE,  // 5CE1
        UNICODE_MODE_MAC,      // 5CE2
        UNICODE_MODE_LNX,      // 5CE3
        UNICODE_MODE_WIN,      // 5CE4
        UNICODE_MODE_BSD,      // 5CE5
        UNICODE_MODE_WINC,     // 5CE6

        // Haptic
        HPT_ON,    // 5CE7
        HPT_OFF,   // 5CE8
        HPT_TOG,   // 5CE9
        HPT_RST,   // 5CEA
        HPT_FBK,   // 5CEB
        HPT_BUZ,   // 5CEC
        HPT_MODI,  // 5CED
        HPT_MODD,  // 5CEE
        HPT_CONT,  // 5CEF
        HPT_CONI,  // 5CF0
        HPT_COND,  // 5CF1
        HPT_DWLI,  // 5CF2
        HPT_DWLD,  // 5CF3

        // Space Cadet (continued)
        KC_LCPO,  // 5CF4
        KC_RCPC,  // 5CF5
        KC_LAPO,  // 5CF6
        KC_RAPC,  // 5CF7

        // Combos
        CMB_ON,   // 5CF8
        CMB_OFF,  // 5CF9
        CMB_TOG,  // 5CFA

        // Magic (continued)
        MAGIC_SWAP_LCTL_LGUI,    // 5CFB
        MAGIC_SWAP_RCTL_RGUI,    // 5CFC
        MAGIC_UNSWAP_LCTL_LGUI,  // 5CFD
        MAGIC_UNSWAP_RCTL_RGUI,  // 5CFE
        MAGIC_SWAP_CTL_GUI,      // 5CFF
        MAGIC_UNSWAP_CTL_GUI,    // 5D00
        MAGIC_TOGGLE_CTL_GUI,    // 5D01
        MAGIC_EE_HANDS_LEFT,     // 5D02
        MAGIC_EE_HANDS_RIGHT,    // 5D03

        // Dynamic Macros
        DYN_REC_START1,   // 5D04
        DYN_REC_START2,   // 5D05
        DYN_REC_STOP,     // 5D06
        DYN_MACRO_PLAY1,  // 5D07
        DYN_MACRO_PLAY2,  // 5D08

        // Joystick
        JS_BUTTON0,   // 5D09
        JS_BUTTON1,   // 5D0A
        JS_BUTTON2,   // 5D0B
        JS_BUTTON3,   // 5D0C
        JS_BUTTON4,   // 5D0D
        JS_BUTTON5,   // 5D0E
        JS_BUTTON6,   // 5D0F
        JS_BUTTON7,   // 5D10
        JS_BUTTON8,   // 5D11
        JS_BUTTON9,   // 5D12
        JS_BUTTON10,  // 5D13
        JS_BUTTON11,  // 5D14
        JS_BUTTON12,  // 5D15
        JS_BUTTON13,  // 5D16
        JS_BUTTON14,  // 5D17
        JS_BUTTON15,  // 5D18
        JS_BUTTON16,  // 5D19
        JS_BUTTON17,  // 5D1A
        JS_BUTTON18,  // 5D1B
        JS_BUTTON19,  // 5D1C
        JS_BUTTON20,  // 5D1D
        JS_BUTTON21,  // 5D1E
        JS_BUTTON22,  // 5D1F
        JS_BUTTON23,  // 5D20
        JS_BUTTON24,  // 5D21
        JS_BUTTON25,  // 5D22
        JS_BUTTON26,  // 5D23
        JS_BUTTON27,  // 5D24
        JS_BUTTON28,  // 5D25
        JS_BUTTON29,  // 5D26
        JS_BUTTON30,  // 5D27
        JS_BUTTON31,  // 5D28

        // Leader Key
        KC_LEAD,  // 5D29

        // Bluetooth: output selection (continued)
        OUT_BT,  // 5D2A

        // Lock Key
        KC_LOCK,  // 5D2B

        // Terminal
        TERM_ON,   // 5D2C
        TERM_OFF,  // 5D2D

        // Sequencer
        SQ_ON,   // 5D2E
        SQ_OFF,  // 5D2F
        SQ_TOG,  // 5D30

        SQ_TMPD,  // 5D31
        SQ_TMPU,  // 5D32

        SQ_RESD,  // 5D33
        SQ_RESU,  // 5D34

        SQ_SALL,  // 5D35
        SQ_SCLR,  // 5D36
    };
}

enum MyKeycodes : ushort
{
    EASYSHIFT = 0x5FFF,
    MYCKC_ESC = 0x6000, //6000 - 5DA6 => 602 Keys before 
    MYCKC_F1,
    MYCKC_F2,
    MYCKC_F3,
    MYCKC_F4,
    MYCKC_F5,
    MYCKC_F6,
    MYCKC_F7,
    MYCKC_F8,
    MYCKC_F9,
    MYCKC_F10,
    MYCKC_F11,
    MYCKC_F12,
    MYCKC_DEL,
    MYCKC_HOME,
    MYCKC_END,
    MYCKC_PGUP,
    MYCKC_PGDN,
    MYCKC_RGB,
    MYCKC_GRV,
    MYCKC_1,
    MYCKC_2,
    MYCKC_3,
    MYCKC_4,
    MYCKC_5,
    MYCKC_6,
    MYCKC_7,
    MYCKC_8,
    MYCKC_9,
    MYCKC_0,
    MYCKC_MINS,
    MYCKC_EQL,
    MYCKC_BSPC,
    MYCKC_NLCK,
    MYCKC_PSLS,
    MYCKC_PAST,
    MYCKC_PMNS,
    MYCKC_TAB,
    MYCKC_Q,
    MYCKC_W,
    MYCKC_E,
    MYCKC_R,
    MYCKC_T,
    MYCKC_Y,
    MYCKC_U,
    MYCKC_I,
    MYCKC_O,
    MYCKC_P,
    MYCKC_LBRC,
    MYCKC_RBRC,
    MYCKC_P7,
    MYCKC_P8,
    MYCKC_P9,
    MYCKC_PPLS,
    MYCKC_CAPS,
    MYCKC_A,
    MYCKC_S,
    MYCKC_D,
    MYCKC_F,
    MYCKC_G,
    MYCKC_H,
    MYCKC_J,
    MYCKC_K,
    MYCKC_L,
    MYCKC_SCLN,
    MYCKC_QUOT,
    MYCKC_NUHS,
    MYCKC_ENT,
    MYCKC_P4,
    MYCKC_P5,
    MYCKC_P6,
    MYCKC_LSFT,
    MYCKC_NUBS,
    MYCKC_Z,
    MYCKC_X,
    MYCKC_C,
    MYCKC_V,
    MYCKC_B,
    MYCKC_N,
    MYCKC_M,
    MYCKC_COMM,
    MYCKC_DOT,
    MYCKC_SLSH,
    MYCKC_RSFT,
    MYCKC_UP,
    MYCKC_P1,
    MYCKC_P2,
    MYCKC_P3,
    MYCKC_PENT,
    MYCKC_LCTL,
    MYCKC_LGUI,
    MYCKC_LALT,
    MYCKC_SPC,
    MYCKC_RALT,
    MYCKC_FN,
    MYCKC_RCTL,
    MYCKC_LEFT,
    MYCKC_DOWN,
    MYCKC_RGHT,
    MYCKC_P0,
    MYCKC_NUMCOL
};

enum LayerNames { _BASE = 0, _FL, _MYCKC = 15 };