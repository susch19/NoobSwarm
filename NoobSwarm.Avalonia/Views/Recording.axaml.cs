using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using NoobSwarm.Avalonia.Controls;
using NoobSwarm.Avalonia.ViewModels;

namespace NoobSwarm.Avalonia.Views
{
    public class Recording : UserControl
    {
        public Recording()
        {
            InitializeComponent();

            DataContextChanged += Recording_DataContextChanged;

        }

        private void Recording_DataContextChanged(object? sender, System.EventArgs e)
        {
            var recordingMakro = this.FindControl<RecordKeysControl>("RecordingMakro");
            var recordingHotKey = this.FindControl<RecordKeysControl>("RecordingHotkey");
            if (recordingMakro is not null
                && recordingHotKey is not null
                && DataContext is RecordingViewModel recordingViewModel)
            {
                recordingHotKey.RecordingCleared += (s, e) =>
                  recordingViewModel.HotkeyRecordingClearedCommand?.Execute(null);
                recordingHotKey.RecordingStarted += (s, e) =>
                    recordingViewModel.HotkeyRecordingStartedCommand?.Execute(null);
                recordingHotKey.RecordingStopped += (s, e) =>
                    recordingViewModel.HotkeyRecordingStoppedCommand?.Execute(e);
                recordingHotKey.KeyRecorded += (s, e) =>
                    recordingViewModel.HotkeyKeyRecordedCommand?.Execute(e);

                recordingMakro.RecordingCleared += (s, e) =>
                  recordingViewModel.MakroRecordingClearedCommand?.Execute(null);
                recordingMakro.RecordingStarted += (s, e) =>
                    recordingViewModel.MakroRecordingStartedCommand?.Execute(null);
                recordingMakro.RecordingStopped += (s, e) =>
                    recordingViewModel.MakroRecordingStoppedCommand?.Execute(e);
                recordingMakro.KeyRecorded += (s, e) =>
                    recordingViewModel.MakroKeyRecordedCommand?.Execute(e);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
