//using MessagePack;

using Microsoft.Win32;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;
using NoobSwarm.Windows;

using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public sealed class ViewModelLocator
    {
        private static IVulcanKeyboard vulcanKeyboard;

        public MainViewModel Main => TypeContainer.Get<MainViewModel>();
        public CockpitViewModel CockpitViewModel => TypeContainer.Get<CockpitViewModel>();
        public ThemeDesignerViewModel ThemeDesignerViewModel => TypeContainer.Get<ThemeDesignerViewModel>();
        public RecordingViewModel RecordingViewModel => TypeContainer.Get<RecordingViewModel>();
        public PlaybackViewModel PlaybackViewModel => TypeContainer.Get<PlaybackViewModel>();
        public ToolbarViewModel ToolbarViewModel => TypeContainer.Get<ToolbarViewModel>();
        public TsViewModel TsViewModel => TypeContainer.Get<TsViewModel>();
        public MakroOverviewModel MakroOverviewModel => TypeContainer.Get<MakroOverviewModel>();

        static ViewModelLocator()
        {
            TypeContainer.Register<MainViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<CockpitViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<ThemeDesignerViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<RecordingViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<PlaybackViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<MakroOverviewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<MakroManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<Keyboard, Keyboard>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IKeyboard, Keyboard>(InstanceBehaviour.Singleton);
            TypeContainer.Register<ToolbarViewModel>(InstanceBehaviour.Singleton);
            LowLevelKeyboardHook hook = new LowLevelKeyboardHook();
            TypeContainer.Register(hook);

            //TypeContainer.Register<IVulcanKeyboard>(VulcanKeyboard.Initialize());
            var key = new GenericWindowsVulcanKeyboard(hook);
            TypeContainer.Register<IVulcanKeyboard>(key);

            var service = LightService.Deserialize();
            TypeContainer.Register(service);


            var hkm = HotKeyManager.Deserialize();
            TypeContainer.Register(hkm);
            hook.HookKeyboard();
            hkm.StartedHotkeyMode += (s, e) => { hook.SetSupressKeyPress();  };
            hkm.StoppedHotkeyMode += (s, e) => { hook.SetSupressKeyPress(false); };

            TypeContainer.Register<TsViewModel>(InstanceBehaviour.Singleton);
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            vulcanKeyboard = TypeContainer.Get<IVulcanKeyboard>();
        }

        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock)
                vulcanKeyboard.Connect();
            else if (e.Reason == SessionSwitchReason.SessionLock)
                vulcanKeyboard.Disconnect();
        }

    }
}
