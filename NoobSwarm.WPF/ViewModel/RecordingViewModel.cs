using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Microsoft.Win32;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Commands;
using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;
using NoobSwarm.Windows;
using NoobSwarm.Windows.Commands;
using NoobSwarm.WPF.Dialog;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Vulcan.NET;
//using System.Windows.Input;

using static NoobSwarm.MakroManager;

namespace NoobSwarm.WPF.ViewModel
{
    public class RecordingViewModel : ViewModelBase
    {
        public bool IsRecording { get; set; }
        public bool BlockInput { get; set; }

        public string RecordingText { get; set; }

        private ReadOnlyCollection<LedKey> hkKeys;

        public System.Windows.Input.ICommand AddHotkeyAsClipboardCommand { get; set; }
        public System.Windows.Input.ICommand AddHotkeyAsOpenProgrammCommand { get; set; }
        public System.Windows.Input.ICommand AddHotkeyAsURLCommand { get; set; }
        public bool AddHotkeyAsClipboardEnabled { get; set; }

        private CancellationTokenSource recordingCts;
        private readonly MakroManager makroManager;
        private readonly HotKeyManager hotKey;
        private readonly Keyboard keyboard;

        public RecordingViewModel()
        {

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                PropertyChanged += RecordingViewModel_PropertyChanged;
                makroManager = TypeContainer.Get<MakroManager>();
                hotKey = TypeContainer.Get<HotKeyManager>();
                keyboard = TypeContainer.Get<Keyboard>();
                makroManager.RecordAdded += HotKey_RecordAdded;
                makroManager.RecordingFinished += (s, e) => { IsRecording = false; };
                AddHotkeyAsClipboardCommand = new RelayCommand(SaveAsClipboard);
                AddHotkeyAsOpenProgrammCommand = new RelayCommand(SaveAsOpenProgram);
                AddHotkeyAsURLCommand = new RelayCommand(SaveAsOpenUrl);
            }
        }

        private void SaveAsOpenUrl()
        {
            var inputDialog = new InputDialog("What URL should be opened with this hotkey?");
            if (inputDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputDialog.Answer))
            {
                var url= inputDialog.Answer;
                if (!url[1..8].Contains("://"))
                    url = "https://" + inputDialog.Answer;

                var command = new OpenUrlCommand()
                {
                    Url = url
                };
                hotKey.AddHotKey(hkKeys, command);
                AddHotkeyAsClipboardEnabled = false;
                recordingCts?.Cancel();
            }
        }

        private async void SaveAsOpenProgram()
        {

            var ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                var inputDialog = new InputDialog("What arguments should be passed to the application for starting?\r\nCan be left empty of press cancel if an empty argumentlist is sufficent.");
                var args = string.Empty;
                if (inputDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputDialog.Answer))
                    args = inputDialog.Answer;

                var command = new OpenProgramCommand()
                {
                    Path = ofd.FileName,
                    Args = args
                };
                hotKey.AddHotKey(hkKeys, command);
                AddHotkeyAsClipboardEnabled = false;
                recordingCts?.Cancel();
            }
        }

        private void SaveAsClipboard()
        {
            var command = new ClipboardTextSaveTypeCommand();
            command.GetDataFromClipboard();
            hotKey.AddHotKey(hkKeys, command);
            AddHotkeyAsClipboardEnabled = false;
            recordingCts?.Cancel();
        }

        private void RecordingViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsRecording):
                    if (IsRecording)
                    {
                        recordingCts = new CancellationTokenSource();
                        Record(recordingCts.Token);
                    }
                    else
                    {
                        recordingCts?.Cancel();
                    }
                    break;
            }
        }

        private async void Record(CancellationToken token)
        {
            RecordingText = "Press the Keys for the Hotkey";


            hkKeys = await hotKey.RecordKeys(token);
            AddHotkeyAsClipboardEnabled = true;

            using (var hook = new LowLevelKeyboardHook())
            {
                hook.HookKeyboard();
                hook.SetSupressKeyPress(BlockInput);
                hook.OnKeyPressed += (s, e) =>
                {
                    makroManager.KeyReceived(e, true);

                };
                hook.OnKeyUnpressed += (s, e) => { makroManager.KeyReceived(e, false); };
                RecordingText = "Press the Makro Keys\r\n";
                var recKeys = await makroManager.StartRecording(token);
                if (AddHotkeyAsClipboardEnabled)
                    hotKey.AddHotKey(hkKeys, new MakroHotkeyCommand(recKeys));
                hotKey.Serialize();
                AddHotkeyAsClipboardEnabled = false;
            }
            IsRecording = false;
        }

        private void HotKey_RecordAdded(object sender, RecordKey e)
        {
            RecordingText += $"{e.Key,-10}\t\tPressed: {e.Pressed}\t\tTimeDelay: {e.TimeBeforePress}\r\n";
        }


    }
}
