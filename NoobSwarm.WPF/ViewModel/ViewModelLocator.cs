using CommonServiceLocator;

using GalaSoft.MvvmLight.Ioc;

using NoobSwarm.Lights;
using NoobSwarm.VirtualHID;

using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public sealed class ViewModelLocator
    {
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public CockpitViewModel CockpitViewModel => ServiceLocator.Current.GetInstance<CockpitViewModel>();
        public ThemeDesignerViewModel ThemeDesignerViewModel => ServiceLocator.Current.GetInstance<ThemeDesignerViewModel>();
        public RecordingViewModel RecordingViewModel => ServiceLocator.Current.GetInstance<RecordingViewModel>();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<CockpitViewModel>();
            SimpleIoc.Default.Register<ThemeDesignerViewModel>();
            SimpleIoc.Default.Register<RecordingViewModel>();
            SimpleIoc.Default.Register<MakroManager>();
            SimpleIoc.Default.Register<Keyboard>();

            SimpleIoc.Default.Register(() => VulcanKeyboard.Initialize());
            SimpleIoc.Default.Register(() => new LightService(ServiceLocator.Current.GetInstance<VulcanKeyboard>()));
            SimpleIoc.Default.Register(() => new HotKeyManager(ServiceLocator.Current.GetInstance<VulcanKeyboard>(), ServiceLocator.Current.GetInstance<LightService>()));
          
        }

     
    }
}
