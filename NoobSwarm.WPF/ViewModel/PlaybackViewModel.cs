using GalaSoft.MvvmLight;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;
using NoobSwarm.Windows;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace NoobSwarm.WPF.ViewModel
{
    public class PlaybackViewModel : ViewModelBase
    {
        public bool UseWinApi { get; set; }

        private readonly HotKeyManager hotKey;
        private readonly Keyboard keyboard;

        public PlaybackViewModel()
        {

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                PropertyChanged += RecordingViewModel_PropertyChanged;
                hotKey = TypeContainer.Get<HotKeyManager>();
                keyboard = TypeContainer.Get<Keyboard>();
            }
        }

        private void RecordingViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(UseWinApi):
                    keyboard.PreferWinApi = UseWinApi;
                    break;
            }
        }

    }
}
