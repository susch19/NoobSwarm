using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using NonSucking.Framework.Extension.IoC;
using NoobSwarm.GenericKeyboard;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace NoobSwarm.Avalonia.Controls
{
    public class RecordKeysControl : UserControl
    {
        public RecordKeysControl()
        {
            InitializeComponent();

            keyBox = this.FindControl<TextBox>("KeyBox");
            keyBox.GetObservable(TextBox.TextProperty).Subscribe(KeyBox_TextChanged);
            ClearCommand = ReactiveCommand.Create(Clear);
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public string StartRecordingText
        {
            get => startRecordingText;
            set => SetAndRaise(StartRecordingTextProperty, ref startRecordingText, value);
        }

        public static readonly DirectProperty<RecordKeysControl, string> StartRecordingTextProperty =
            AvaloniaProperty.RegisterDirect<RecordKeysControl, string>(nameof(StartRecordingText),
                r => r.StartRecordingText, (r, v) => r.StartRecordingText = v);

        public string StopRecordingText
        {
            get => stopRecordingText;
            set => SetAndRaise(StopRecordingTextProperty, ref stopRecordingText, value);
        }


        public static readonly DirectProperty<RecordKeysControl, string> StopRecordingTextProperty =
            AvaloniaProperty.RegisterDirect<RecordKeysControl, string>(nameof(StopRecordingText),
                r => r.StopRecordingText, (r, v) => r.StopRecordingText = v);


        public bool BlockInput
        {
            get => blockInput;
            set
            {
                SetAndRaise(BlockInputProperty, ref blockInput, value);
                hook?.SetSupressKeyPress(value);
            }
        }

        public static readonly DirectProperty<RecordKeysControl, bool> BlockInputProperty =
            AvaloniaProperty.RegisterDirect<RecordKeysControl, bool>(nameof(BlockInput), r => r.BlockInput,
                (r, v) => r.BlockInput = v);


        public ObservableCollection<MakroManager.RecordKey> RecordedKeys
        {
            get => recordedKeys;
            set
            {
                if (value is null)
                    return;
                SetAndRaise(RecordedKeysProperty, ref recordedKeys, value);
                SetRecordedKeys(value);
            }
        }

        public static readonly DirectProperty<RecordKeysControl, ObservableCollection<MakroManager.RecordKey>>
            RecordedKeysProperty =
                AvaloniaProperty.RegisterDirect<RecordKeysControl, ObservableCollection<MakroManager.RecordKey>>(
                    nameof(RecordedKeys), r => r.RecordedKeys, (r, v) => r.RecordedKeys = v);


        public ICommand ClearCommand
        {
            get => clearCommand;
            set => SetAndRaise(ClearCommandProperty, ref clearCommand, value);
        }

        public static readonly DirectProperty<RecordKeysControl, ICommand> ClearCommandProperty =
            AvaloniaProperty.RegisterDirect<RecordKeysControl, ICommand>(nameof(ClearCommand), r => r.ClearCommand,
                (r, v) => r.ClearCommand = v);

        public RecordKeysPrintMode PrintMode
        {
            get => printMode;
            set => SetAndRaise(PrintModeProperty, ref printMode, value);
        }


        public static readonly DirectProperty<RecordKeysControl, RecordKeysPrintMode> PrintModeProperty =
            AvaloniaProperty.RegisterDirect<RecordKeysControl, RecordKeysPrintMode>(nameof(PrintMode), r => r.PrintMode,
                (r, v) => r.PrintMode = v);

        public class RecordingStoppedEventArgs : EventArgs
        {
            public IReadOnlyCollection<MakroManager.RecordKey> RecordedKeys { get; set; }

            public RecordingStoppedEventArgs(IReadOnlyCollection<MakroManager.RecordKey> recordedKeys)
            {
                RecordedKeys = recordedKeys;
            }
        }

        public class KeyRecordedEventArgs : EventArgs
        {
            public MakroManager.RecordKey RecordedKey { get; set; }

            public KeyRecordedEventArgs(MakroManager.RecordKey recordedKey)
            {
                RecordedKey = recordedKey;
            }
        }

        public event EventHandler RecordingStarted;
        public event EventHandler<RecordingStoppedEventArgs> RecordingStopped;
        public event EventHandler<KeyRecordedEventArgs> KeyRecorded;
        public event EventHandler RecordingCleared;

        private ObservableCollection<MakroManager.RecordKey> recordedKeys = new();
        private KeyboardHook hook;
        private MakroManager makroManager;
        private string startRecordingText;
        private string stopRecordingText;
        private bool blockInput;
        private readonly TextBox keyBox;

        private ICommand clearCommand;
        private RecordKeysPrintMode printMode;


        private void SetRecordedKeys(ObservableCollection<MakroManager.RecordKey> keys)
        {
            recordedKeys.Clear();
            foreach (var key in keys)
                recordedKeys.Add(key);
            UpdateText(keys.ToArray());
            makroManager?.Clear(keys);
        }

        private void UpdateText(params MakroManager.RecordKey[] keys)
        {
            if (keys is null)
                return;

            Dispatcher.UIThread.Post(() =>
            {
                switch (PrintMode)
                {
                    case RecordKeysPrintMode.Normal:
                        keyBox.Text += $" {string.Join(" ", keys.Where(x => x.Pressed).Select(x => $"{x.Key}"))}";
                        break;

                    case RecordKeysPrintMode.Full:
                        keyBox.Text +=
                            $" {string.Join(" ", keys.Where(x => x.Pressed).Select(x => $"{x.Key,-10}\t\tPressed: {x.Pressed}\t\tTimeDelay: {x.TimeBeforePress}\r\n"))}";
                        break;
                }
            });
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // e.Handled = true;
        }

        private void TextBox_GotFocus(object sender, GotFocusEventArgs e)
        {
            //HintAssist.SetHint(this, StopRecordingText);

            if (hook is null)
            {
                makroManager = new MakroManager();
                makroManager.RecordAdded += MakroManager_RecordAdded;
                makroManager.RecordingFinished += MakroManager_RecordingFinished;

                hook = TypeContainer.Get<KeyboardHook>();
                hook.OnKeyPressed += Hook_OnKeyPressed;
                hook.OnKeyUnpressed += Hook_OnKeyUnpressed;
                hook.SetSupressKeyPress(BlockInput);
                RecordingStarted?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Hook_OnKeyUnpressed(object sender, Makros.Key e)
        {
            makroManager.KeyReceived(e, false);
        }

        private void Hook_OnKeyPressed(object sender, Makros.Key e)
        {
            if (recordedKeys.Count == 0)
            {
                makroManager.BeginRecording(recordedKeys.ToList());
            }

            makroManager.KeyReceived(e, true);
        }

        private void MakroManager_RecordAdded(object sender, MakroManager.RecordKey e)
        {
            recordedKeys.Add(e);
            UpdateText(new[] {e});
            Dispatcher.UIThread.Post(() => KeyRecorded?.Invoke(this, new(e)));
        }

        private void MakroManager_RecordingFinished(object sender, IReadOnlyCollection<MakroManager.RecordKey> e)
        {
            Dispatcher.UIThread.Post(() => RecordingStopped?.Invoke(this, new(e)));
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //HintAssist.SetHint(this, StartRecordingText);

            if (hook is not null)
            {
                try
                {
                    makroManager.FinishRecording();
                    makroManager.RecordAdded -= MakroManager_RecordAdded;
                    makroManager.RecordingFinished -= MakroManager_RecordingFinished;
                    makroManager.Dispose();
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }

                makroManager = null;

                try
                {
                    hook.OnKeyPressed -= Hook_OnKeyPressed;
                    hook.OnKeyUnpressed -= Hook_OnKeyUnpressed;
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }

                hook = null;
            }
        }

        private void KeyBox_TextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Clear();
            }
        }

        public void Clear()
        {
            recordedKeys.Clear();
            makroManager?.Clear();
            keyBox.Text = "";
            RecordingCleared?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum RecordKeysPrintMode
    {
        Normal,
        Full
    }
}