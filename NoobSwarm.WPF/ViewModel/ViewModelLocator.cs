﻿
using GalaSoft.MvvmLight.Ioc;

using MessagePack;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights;
using NoobSwarm.Makros;
using NoobSwarm.MessagePackFormatters;
using NoobSwarm.VirtualHID;

using System;

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
            InitializeMessagePack();
            //ServiceLocator.SetLocatorProvider(() => typeContainer);

            TypeContainer.Register<MainViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<CockpitViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<ThemeDesignerViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<RecordingViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register<MakroManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<Keyboard, Keyboard>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IKeyboard, Keyboard>(InstanceBehaviour.Singleton);
            TypeContainer.Register<ToolbarViewModel>(InstanceBehaviour.Singleton);
            TypeContainer.Register(VulcanKeyboard.Initialize());
            TypeContainer.Register<LightService>(InstanceBehaviour.Singleton);
            var hkm = HotKeyManager.Deserialize();
            TypeContainer.Register(hkm);

          

        }

        private void InitializeMessagePack()
        {
            MessagePack.Resolvers.StaticCompositeResolver.Instance.Register(
         new SystemDrawingColorFormatter());

            var compResolver = MessagePack.Resolvers.CompositeResolver.Create(
                MessagePack.Resolvers.StandardResolver.Instance,
                MessagePack.Resolvers.StaticCompositeResolver.Instance);
            var defaultOptions = MessagePackSerializerOptions.Standard.WithResolver(compResolver);

            MessagePack.MessagePackSerializer.DefaultOptions = defaultOptions;
        }
    }
}
