using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;
using NoobSwarm.Windows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public class CommandWrapper
    {
        public List<LedKey> Keys { get; }
        public MakroHotkeyCommand Command { get; }

        public CommandWrapper(List<LedKey> keys, MakroHotkeyCommand command)
        {
            Keys = keys;
            Command = command;
        }
    }

    public class PlaybackViewModel : ViewModelBase
    {
        public bool UseWinApi { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ObservableCollection<CommandWrapper> Commands { get; set; }
        public CommandWrapper? SelectedCommand { get; set; }

        private readonly HotKeyManager hotKey;
        private readonly VirtualHID.Keyboard keyboard;


        public PlaybackViewModel()
        {

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                Commands = new ObservableCollection<CommandWrapper>();
                PropertyChanged += RecordingViewModel_PropertyChanged;
                hotKey = TypeContainer.Get<HotKeyManager>();
                keyboard = TypeContainer.Get<VirtualHID.Keyboard>();
                RefreshCommand = new RelayCommand(RefreshMakros);
            }
        }

        private void RefreshMakros()
        {
            Commands.Clear();

            foreach (var item in hotKey.GetHotkeys().ToList())
            {
                if (item.command is MakroHotkeyCommand mhc)
                    Commands.Add(new (item.keys, mhc));
            }
            SelectedCommand = Commands.FirstOrDefault();
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
