using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        public ICommand LoadedCommand { get; set; }
        public ICommand UnloadedCommand { get; set; }

        public bool IsRecording { get; set; }

        public string RecordingText { get; set; }

        private VulcanKeyboard keyboard;
        private HotKeyManager hotKey;
        private CancellationTokenSource recordingCts;

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                LoadedCommand = new RelayCommand(Loaded);
                UnloadedCommand = new RelayCommand(Unloaded);
                PropertyChanged += MainViewModel_PropertyChanged;
            }
        }

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsRecording))
            {
                if (IsRecording)
                {
                    recordingCts = new CancellationTokenSource();
                    Record(recordingCts.Token);
                }
                else
                {
                    recordingCts?.Cancel();
                }
            }
        }

        private void Loaded()
        {
            keyboard = VulcanKeyboard.Initialize();
            hotKey = new(keyboard, LedKey.FN_Key);
        }

        private void Unloaded()
        {
            keyboard?.Dispose();
        }

        private async void Record(CancellationToken token)
        {
            RecordingText = "";
            try
            {
                await foreach (var item in hotKey.Record(token))
                    RecordingText += " " + item.ToString();
            }
            catch (TaskCanceledException) { }
            IsRecording = false;
        }
    }
}
