using Avalonia.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Avalonia.Views;

using ReactiveUI;

using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Input;

namespace NoobSwarm.Avalonia.ViewModels
{
    [ClassPropertyChangedAvalonia(typeof(MainWindowViewModelProps))]

    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly HotKeyManager manager;
        private HotkeyWindow? hotkeyWindow;

        private class MainWindowViewModelProps
        {
            public RecordingViewModel RecordingViewModel { get; set; }

        }


        public MainWindowViewModel()
        {
            RecordingViewModel = new();
            manager = TypeContainer.Get<HotKeyManager>();
            Startup();
        }

        private void Startup()
        {
            manager.StartedHotkeyMode += Hkm_StartedHotkeyMode;
            manager.StoppedHotkeyMode += Hkm_StoppedHotkeyMode;
        }

        private void Hkm_StartedHotkeyMode(object sender, EventArgs e)
        {
            bool flag = hotkeyWindow == null;
            if (flag)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    hotkeyWindow = new HotkeyWindow(manager);
                    hotkeyWindow.Show();
                });
            }
        }

        private void Hkm_StoppedHotkeyMode(object sender, EventArgs e)
        {
            bool flag = hotkeyWindow != null;
            if (flag)
            {
                Dispatcher.UIThread.Post(delegate ()
                {
                    HotkeyWindow hotkeyWindow = this.hotkeyWindow;
                    if (hotkeyWindow != null)
                    {
                        hotkeyWindow.Close();
                    }
                    this.hotkeyWindow = null;
                });
            }
        }
    }
}
