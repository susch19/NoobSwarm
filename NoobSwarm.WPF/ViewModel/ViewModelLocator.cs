
using GalaSoft.MvvmLight.Ioc;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights;
using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;

using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public sealed class ViewModelLocator
    {
        public MainViewModel Main => TypeContainer.Get<MainViewModel>();
        public CockpitViewModel CockpitViewModel => TypeContainer.Get<CockpitViewModel>();
        public ThemeDesignerViewModel ThemeDesignerViewModel => TypeContainer.Get<ThemeDesignerViewModel>();
        public RecordingViewModel RecordingViewModel => TypeContainer.Get<RecordingViewModel>();
        public ToolbarViewModel ToolbarViewModel => TypeContainer.Get<ToolbarViewModel>();

        public ViewModelLocator()
        {

            //ServiceLocator.SetLocatorProvider(() => typeContainer);

            TypeContainer.Register<MainViewModel>();
            TypeContainer.Register<CockpitViewModel>();
            TypeContainer.Register<ThemeDesignerViewModel>();
            TypeContainer.Register<RecordingViewModel>();
            TypeContainer.Register<MakroManager>();
            TypeContainer.Register<Keyboard, Keyboard>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IKeyboard, Keyboard>(InstanceBehaviour.Singleton);
            TypeContainer.Register<ToolbarViewModel>();
            TypeContainer.Register(VulcanKeyboard.Initialize());
            TypeContainer.Register(new LightService(TypeContainer.Get<VulcanKeyboard>()));
            TypeContainer.Register(new HotKeyManager(TypeContainer.Get<VulcanKeyboard>(), TypeContainer.Get<LightService>()));
          
        }

     
    }
}
