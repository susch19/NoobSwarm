using CommonServiceLocator;

using GalaSoft.MvvmLight;

using NoobSwarm.VirtualHID;
using NoobSwarm.Windows;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static NoobSwarm.MakroManager;

namespace NoobSwarm.WPF.ViewModel
{
    public class RecordingViewModel : ViewModelBase
    {
        public bool IsRecording { get; set; }
        public bool BlockInput { get; set; }

        public string RecordingText { get; set; }

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
                makroManager = ServiceLocator.Current.GetInstance<MakroManager>();
                hotKey = ServiceLocator.Current.GetInstance<HotKeyManager>();
                keyboard = ServiceLocator.Current.GetInstance<Keyboard>();
                makroManager.RecordAdded += HotKey_RecordAdded;
                makroManager.RecordingFinished += (s, e) => { IsRecording = false; };
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
            RecordingText = "Press the Keys for the Hotkey";


            var hkKeys = await hotKey.RecordKeys(token);


            using (var hook = new LowLevelKeyboardHook())
            {
                hook.HookKeyboard();
                hook.SetSupressKeyPress(BlockInput);
                hook.OnKeyPressed += (s, e) =>
                {
                    makroManager.KeyReceived((Makros.Key)e, true);

                };
                hook.OnKeyUnpressed += (s, e) => { makroManager.KeyReceived((Makros.Key)e, false); };
                RecordingText = "Press the Makro Keys\r\n";
                var recKeys = await makroManager.StartRecording(token);
                hotKey.AddHotKey(hkKeys, (vk) => { keyboard.PlayMacro(recKeys); });
            }
            IsRecording = false;
        }

        private void HotKey_RecordAdded(object sender, RecordKey e)
        {
            RecordingText += $"{e.Key,-10}\t\tPressed: {e.Pressed}\t\tTimeDelay: {e.TimeBeforePress}\r\n";
        }


    }
}
