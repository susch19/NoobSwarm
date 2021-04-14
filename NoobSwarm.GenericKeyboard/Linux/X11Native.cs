using System;
using System.Runtime.InteropServices;
using Window = System.UInt64;
using Time = System.UInt64;
using Bool = System.Int32;
namespace NoobSwarm.GenericKeyboard.Linux
{
    public class X11Native
    {
        public const int GrabModeAsync = 1;
        public const uint LockMask = 0x2;
        public const uint Mod2Mask = 0x10;
        public const uint Mod3Mask = 0x20;
        public const uint Mod5Mask = 0x80;

        public const long KeyPressMask = 1;

        public const int KeyPress = 2;

        public const int KeyRelease = 3;
        public const ulong CurrentTime = 0;
        
        public unsafe struct XKeyEvent{
            public int type;
            public ulong serial;
            public int send_event;
            public IntPtr display;
            public Window window;
            public Window root;
            public Window subwindow;
            public Time time;
            public int x, y;
            public int x_root, y_root;
            public uint state;
            public uint keycode;
            public int same_screen;

            public fixed byte padding[96];
        }


        [DllImport("X11")]
        public static extern IntPtr XOpenDisplay(IntPtr display_name);

        [DllImport("X11")]
        public static extern int XCloseDisplay(IntPtr display);

        public static unsafe Window DefaultRootWindow(IntPtr display)
        {
            var privDisp = (_XPrivDisp*) display;
            var defaultScreen = privDisp->default_screen;
            var screenBase = (Screen*)privDisp->screens;
            Window root =(screenBase[defaultScreen].root);
            return root;
        }
        
        [DllImport("X11")]
        public static extern int XGrabKey(IntPtr display, int keycode, uint modifiers, Window grab_window, Bool owner_events, int pointer_mode, int keyboard_mode);
                
        [DllImport("X11")]
        public static extern int XUngrabKey(IntPtr display, int keycode, uint modifiers, Window grab_window);

        [DllImport("X11")]
        public static extern int XGrabKeyboard(IntPtr display, Window grab_window, Bool owner_events,
            int pointer_mode, int keyboard_mode, Time time);
        
        [DllImport("X11")]
        public static extern int XUngrabKeyboard(IntPtr display, Time time);

        [DllImport("X11")]
        public static extern byte XKeysymToKeycode(IntPtr display, ulong keysym);
        
        [DllImport("X11")]
        public static extern int XSelectInput(IntPtr display, Window w, long event_mask);
        
        [DllImport("X11")]
        public static extern int XNextEvent(IntPtr display, ref XKeyEvent event_return);
        
        [DllImport("X11")]
        public static extern ulong XLookupKeysym(ref XKeyEvent key_event, int index);
        
        
        
        
        
        
        
        
        
        
        
        
        unsafe struct _XPrivDisp
        {
            public IntPtr ext_data;//XExtData *ext_data;
            private IntPtr private1;//struct _XPrivate *private1;
            public int fd;
            public int private2;
            public int proto_major_version;
            public int proto_minor_version;
            private IntPtr vendor;//char *vendor;
            private ulong private3;// XID private3;
            private ulong private4;// XID private4;
            private ulong private5;// XID private5;
            public int private6;
            public IntPtr resource_alloc;//XID (*resource_alloc)(struct _XDisplay*);
            public int byte_order;
            public int bitmap_unit;
            public int bitmap_pad;
            public int bitmap_bit_order;
            public int nformats;
            public IntPtr pixmap_format;//ScreenFormat *pixmap_format;
            public int private8;
            public int release;
            public IntPtr private9, private10;//struct _XPrivate *private9, *private10;
            public int qlen;
            public ulong last_request_read;
            public ulong request;
            private IntPtr private11;// XPointer private11;
            private IntPtr private12;// XPointer private12;
            private IntPtr private13;// XPointer private13;
            private IntPtr private14;// XPointer private14;
            public uint max_request_size;
            public IntPtr db;
            public IntPtr private15;//int (*private15)(struct _XDisplay*);
            public IntPtr display_name;//char *display_name;
            public int default_screen;
            public int nscreens;
            public IntPtr screens;
            public ulong motion_buffer;
            public ulong private16;
            public int min_keycode;
            public int max_keycode;
            private IntPtr private17;//XPointer private17;
            private IntPtr private18;//XPointer private18;
            public int private19;
            public IntPtr xdefaults; //public char *xdefaults;
        }
        
        struct Screen
        {
            public IntPtr ext_data;//XExtData *ext_data;
            public IntPtr display;//struct _XDisplay *display;
            public Window root;
            public int width, height;
            public int mwidth, mheight;
            public int ndepths;
            public IntPtr depths;//public Depth *depths;
            public int root_depth;
            public IntPtr root_visual;//public Visual *root_visual;
            public IntPtr default_gc;//public GC default_gc;
            public ulong cmap;//public Colormap cmap;
            public ulong white_pixel;
            public ulong black_pixel;
            public int max_maps, min_maps;
            public int backing_store;
            public int save_unders;
            public long root_input_mask;
        }
    }
}