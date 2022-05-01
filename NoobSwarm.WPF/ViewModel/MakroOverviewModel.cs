using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Hotkeys;
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


    public class MakroOverviewModel : ViewModelBase
    {
        public bool UseWinApi { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ObservableCollection<CommandTypeWrapper> Commands { get; set; }
        public CommandTypeWrapper? SelectedCommand { get; set; }

        private readonly HotKeyManager hotKey;
        private readonly VirtualHID.Keyboard keyboard;


        public MakroOverviewModel()
        {

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                Commands = new ObservableCollection<CommandTypeWrapper>();
                hotKey = TypeContainer.Get<HotKeyManager>();
                keyboard = TypeContainer.Get<VirtualHID.Keyboard>();
                RefreshCommand = new RelayCommand(RefreshMakros);
                DeleteCommand = new RelayCommand<CommandTypeWrapper>(DeleteMakro);
                EditCommand = new RelayCommand<CommandTypeWrapper>(EditMakro);
                RefreshMakros();
            }
        }

        private void DeleteMakro(CommandTypeWrapper obj)
        {
            if (hotKey.DeleteHotKey(obj.Keys))
                Commands.Remove(obj);
            hotKey.Serialize();
        }
        private void EditMakro(CommandTypeWrapper obj)
        {
            if (!obj.Command.Editable)
                return;

        }

        private void RefreshMakros()
        {
            Commands.Clear();

            foreach (var item in hotKey.GetHotkeys().OrderBy(x => x.keys[0].ToString()).ToList())
            {
                Commands.Add(new(item.keys, item.command));
            }
            SelectedCommand = Commands.FirstOrDefault();
        }

        public class CommandTypeWrapper
        {
            public List<LedKey> Keys { get; }
            public IHotkeyCommand Command { get; set; }
            public string TypeName { get; }

            public CommandTypeWrapper(List<LedKey> keys, IHotkeyCommand command)
            {
                Keys = keys;
                Command = command;
                TypeName = command.GetType().Name;
            }
        }
    }
}
