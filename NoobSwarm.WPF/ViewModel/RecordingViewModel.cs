using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Commands;
using NoobSwarm.Hotkeys;
using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;
using NoobSwarm.Windows;
using NoobSwarm.Windows.Commands;
using NoobSwarm.WPF.Dialog;
using NoobSwarm.WPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Vulcan.NET;
using static NoobSwarm.MakroManager;
using Keyboard = NoobSwarm.VirtualHID.Keyboard;

namespace NoobSwarm.WPF.ViewModel
{
    public class RecordingViewModel : ViewModelBase
    {
        #region HotKey
        public ICommand HotkeyRecordingClearedCommand { get; set; }
        public ICommand HotkeyRecordingStartedCommand { get; set; }
        public ICommand HotkeyRecordingStoppedCommand { get; set; }
        public ICommand HotkeyKeyRecordedCommand { get; set; }
        public ICommand HotkeyClearCommand { get; set; }
        public ObservableCollection<RecordKey> HotkeyRecordedKeys { get; set; }
        #endregion

        #region Makro
        public ICommand MakroRecordingClearedCommand { get; set; }
        public ICommand MakroRecordingStartedCommand { get; set; }
        public ICommand MakroRecordingStoppedCommand { get; set; }
        public ICommand MakroKeyRecordedCommand { get; set; }
        public ICommand MakroClearCommand { get; set; }
        public ObservableCollection<RecordKey> MakroRecordedKeys { get; set; }
        #endregion



        public ICommand SaveCommand { get; set; }

        public bool BlockInput { get; set; }

        public bool HotkeyCommandEnabled { get; set; }

        public ICommand AddHotkeyAsClipboardCommand { get; set; }
        public ICommand AddHotkeyAsOpenProgrammCommand { get; set; }
        public ICommand AddHotkeyAsURLCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        private readonly MakroManager makroManager;
        private readonly HotKeyManager hotKey;
        private readonly Keyboard keyboard;
        private IReadOnlyList<RecordKey> hotKeys;
        private IHotkeyCommand Command
        {
            get => command;
            set
            {
                command = value;
                HotkeyCommandEnabled = command == null;
            }
        }
        private IHotkeyCommand command;

        public RecordingViewModel()
        {

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                HotkeyCommandEnabled = true;
                makroManager = TypeContainer.Get<MakroManager>();
                hotKey = TypeContainer.Get<HotKeyManager>();
                keyboard = TypeContainer.Get<Keyboard>();

                SaveCommand = new RelayCommand(Save);
                HotkeyRecordingStoppedCommand = new RelayCommand<RecordKeysControl.RecordingStoppedEventArgs>(HotkeyRecordingStopped);
                MakroRecordingStoppedCommand = new RelayCommand<RecordKeysControl.RecordingStoppedEventArgs>(MakroRecordingStopped);

                AddHotkeyAsClipboardCommand = new RelayCommand(SaveAsClipboard);
                AddHotkeyAsOpenProgrammCommand = new RelayCommand(SaveAsOpenProgram);
                AddHotkeyAsURLCommand = new RelayCommand(SaveAsOpenUrl);
                ClearCommand = new RelayCommand(Clear);
            }
        }

        private void MakroRecordingStopped(RecordKeysControl.RecordingStoppedEventArgs e)
        {
            if (e.RecordedKeys.Count == 0)
                return;

            Command = new MakroHotkeyCommand(e.RecordedKeys.ToList());
        }

        private void HotkeyRecordingStopped(RecordKeysControl.RecordingStoppedEventArgs e)
        {
            if (e.RecordedKeys.Count == 0)
                return;

            hotKeys = new ReadOnlyCollection<RecordKey>(e.RecordedKeys.ToList());
        }

        private void Save()
        {
            if (hotKeys?.Any() != true || Command is null)
                return;

            // TODO: Add name
            hotKey.AddHotKey(new ReadOnlyCollection<LedKey>(hotKeys.Where(x => x.Pressed).Select(x => LedKeyToKeyMapper.KeyToLedKey[x.Key]).ToList()), Command);
            hotKey.Serialize();

            Clear();
        }

        private void Clear()
        {
            hotKeys = new ReadOnlyCollection<RecordKey>(Array.Empty<RecordKey>());
            Command = null;

            HotkeyClearCommand?.Execute(null);
            MakroClearCommand?.Execute(null);
        }

        #region Commands

        private void SaveAsOpenUrl()
        {
            var inputDialog = new InputDialog("What URL should be opened with this hotkey?");
            if (inputDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputDialog.Answer))
            {
                var url = inputDialog.Answer;
                if (!url[1..8].Contains("://"))
                    url = "https://" + inputDialog.Answer;

                Command = new OpenUrlCommand()
                {
                    Url = url
                };
            }
        }

        private void SaveAsOpenProgram()
        {
            var ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                var inputDialog = new InputDialog("What arguments should be passed to the application for starting?\r\nCan be left empty of press cancel if an empty argumentlist is sufficent.");
                var args = string.Empty;
                if (inputDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputDialog.Answer))
                    args = inputDialog.Answer;

                Command = new OpenProgramCommand()
                {
                    Path = ofd.FileName,
                    Args = args
                };
            }
        }

        private void SaveAsClipboard()
        {
            var command = new ClipboardTextSaveTypeCommand();
            command.GetDataFromClipboard();
            Command = command;
        }

        #endregion
    }
}
