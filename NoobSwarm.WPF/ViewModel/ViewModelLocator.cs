//using MessagePack;

using Microsoft.Win32;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights;
using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;

using System.Configuration;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public sealed class ViewModelLocator
    {
        private static VulcanKeyboard vulcanKeyboard;

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
            TypeContainer.Register(VulcanKeyboard.Initialize());
            var service = LightService.Deserialize();
            TypeContainer.Register(service);
            var hkm = HotKeyManager.Deserialize();
            TypeContainer.Register(hkm);
            TypeContainer.Register<TsViewModel>(InstanceBehaviour.Singleton);
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            vulcanKeyboard = TypeContainer.Get<VulcanKeyboard>();
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
