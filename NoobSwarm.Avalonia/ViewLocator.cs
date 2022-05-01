using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Avalonia.ViewModels;
using NoobSwarm.GenericKeyboard;
using NoobSwarm.Lights;
using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;

using System;
using System.Runtime.InteropServices;

using Vulcan.NET;

namespace NoobSwarm.Avalonia
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public ViewLocator()
        {
            TypeContainer.Register<MakroManager>(InstanceBehaviour.Singleton);
            //TypeContainer.Register<Keyboard, Keyboard>(InstanceBehaviour.Singleton);

            //var key = VulcanKeyboard.Initialize();
            //TypeContainer.Register<IVulcanKeyboard>(VulcanKeyboard.Initialize());
            var keyForHook = new GenericVulcanKeyboard();
            TypeContainer.Register(keyForHook.Hook);
            //TypeContainer.Register<IVulcanKeyboard>(key);
            var key = QMKKeyboard.Initialize();
            TypeContainer.Register<IVulcanKeyboard>(key);


            var service = LightService.Deserialize();
            TypeContainer.Register(service);


            var hkm = HotKeyManager.Deserialize();
            hkm.Mode = HotKeyMode.Active;
            TypeContainer.Register(hkm);
            //if (keyForHook.Hook is LowLevelKeyboardHookWindows hook)
            //{
            //    hkm.StartedHotkeyMode += (s, e) => { hook.SetSupressKeyPress(); };
            //    hkm.StoppedHotkeyMode += (s, e) => { hook.SetSupressKeyPress(false); };

            //}

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                StartupLinux();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                StartupWindows();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                throw new NotSupportedException();
        }

        private void StartupWindows()
        {
            TypeContainer.Register<Keyboard, Keyboard>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IKeyboard, Keyboard>(InstanceBehaviour.Singleton);
        }

        private void StartupLinux()
        {
        }

        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
