using CommonServiceLocator;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NoobSwarm.WPF.ViewModel
{
    public class RecordingViewModel : ViewModelBase
    {
        public bool IsRecording { get; set; }

        public string RecordingText { get; set; }

        private CancellationTokenSource recordingCts;
        private readonly HotKeyManager hotKey;

        public RecordingViewModel()
        {

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                PropertyChanged += RecordingViewModel_PropertyChanged;
                hotKey = ServiceLocator.Current.GetInstance<HotKeyManager>();
            }
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
