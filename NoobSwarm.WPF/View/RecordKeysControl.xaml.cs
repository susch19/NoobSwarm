using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using NonSucking.Framework.Extension.IoC;

using NoobSwarm.GenericKeyboard;
using NoobSwarm.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoobSwarm.WPF.View
{
    public enum RecordKeysPrintMode
    {
        Normal,
        Full
    }

    /// <summary>
    /// Interaktionslogik für RecordKeysControl.xaml
    /// </summary>
    public partial class RecordKeysControl : UserControl
    {
        public string StartRecordingText
        {
            get => (string)GetValue(StartRecordingTextProperty);
            set => SetValue(StartRecordingTextProperty, value);
        }

        public static readonly DependencyProperty StartRecordingTextProperty =
            DependencyProperty.Register(nameof(StartRecordingText), typeof(string), typeof(RecordKeysControl), new PropertyMetadata("Click to record"));

        public string StopRecordingText
        {
            get => (string)GetValue(StopRecordingTextProperty);
            set => SetValue(StopRecordingTextProperty, value);
        }

        public static readonly DependencyProperty StopRecordingTextProperty =
            DependencyProperty.Register(nameof(StopRecordingText), typeof(string), typeof(RecordKeysControl), new PropertyMetadata("Recording.."));

        public bool BlockInput
        {
            get => (bool)GetValue(BlockInputProperty);
            set => SetValue(BlockInputProperty, value);
        }

        public static readonly DependencyProperty BlockInputProperty =
            DependencyProperty.Register(nameof(BlockInput), typeof(bool), typeof(RecordKeysControl), new PropertyMetadata(BlockInputChanged));

        private static void BlockInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RecordKeysControl ctl)
                return;

            ctl.hook?.SetSupressKeyPress((bool)e.NewValue);
        }

        public ObservableCollection<MakroManager.RecordKey> RecordedKeys
        {
            get => (ObservableCollection<MakroManager.RecordKey>)GetValue(RecordedKeysProperty);
            set => SetValue(RecordedKeysProperty, value);
        }

        public static readonly DependencyProperty RecordedKeysProperty =
            DependencyProperty.Register(nameof(RecordedKeys), typeof(ObservableCollection<MakroManager.RecordKey>), typeof(RecordKeysControl), new PropertyMetadata(RecordedKeysChanged));

        private static void RecordedKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RecordKeysControl ctl)
                return;

            ctl.SetRecordedKeys((ObservableCollection<MakroManager.RecordKey>)e.NewValue);
        }

        public ICommand ClearCommand
        {
            get { return (ICommand)GetValue(ClearCommandProperty); }
            set { SetValue(ClearCommandProperty, value); }
        }

        private static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register(nameof(ClearCommand), typeof(ICommand), typeof(RecordKeysControl), new PropertyMetadata(null));

        public RecordKeysPrintMode PrintMode
        {
            get { return (RecordKeysPrintMode)GetValue(PrintModeProperty); }
            set { SetValue(PrintModeProperty, value); }
        }

        public static readonly DependencyProperty PrintModeProperty =
            DependencyProperty.Register(nameof(PrintMode), typeof(RecordKeysPrintMode), typeof(RecordKeysControl), new PropertyMetadata(RecordKeysPrintMode.Normal));

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
        private LowLevelKeyboardHookWindows hook;
        private MakroManager makroManager;

        public RecordKeysControl()
        {
            InitializeComponent();
            Loaded += RecordKeysControl_Loaded;
        }

        private void RecordKeysControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetValue(ClearCommandProperty, new RelayCommand(Clear));
            HintAssist.SetHint(this, StartRecordingText);
        }

        private void SetRecordedKeys(ObservableCollection<MakroManager.RecordKey> keys)
        {
            recordedKeys.Clear();
            keys.ForEach(recordedKeys.Add);
            UpdateText(keys.ToArray());
            makroManager?.Clear(keys);
        }

        private void UpdateText(params MakroManager.RecordKey[] keys)
        {
            if (keys is null)
                return;

            Dispatcher.Invoke(() =>
            {
                switch (PrintMode)
                {
                    case RecordKeysPrintMode.Normal:
                        KeyBox.Text += $" {string.Join(" ", keys.Where(x => x.Pressed).Select(x => $"{x.Key}"))}";
                        break;

                    case RecordKeysPrintMode.Full:
                        KeyBox.Text += $" {string.Join(" ", keys.Where(x => x.Pressed).Select(x => $"{x.Key,-10}\t\tPressed: {x.Pressed}\t\tTimeDelay: {x.TimeBeforePress}\r\n"))}";
                        break;
                }
            });
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            HintAssist.SetHint(this, StopRecordingText);

            if (hook is null)
            {
                makroManager = new MakroManager();
                makroManager.RecordAdded += MakroManager_RecordAdded;
                makroManager.RecordingFinished += MakroManager_RecordingFinished;

                hook = TypeContainer.Get<LowLevelKeyboardHookWindows>();
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
            UpdateText(new[] { e });
            Dispatcher.Invoke(() => KeyRecorded?.Invoke(this, new(e)));
        }

        private void MakroManager_RecordingFinished(object sender, IReadOnlyCollection<MakroManager.RecordKey> e)
        {
            Dispatcher.Invoke(() => RecordingStopped?.Invoke(this, new(e)));
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            HintAssist.SetHint(this, StartRecordingText);

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

        private void KeyBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(KeyBox.Text))
            {
                Clear();
            }
        }

        public void Clear()
        {
            recordedKeys.Clear();
            makroManager?.Clear();
            KeyBox.Text = "";
            RecordingCleared?.Invoke(this, EventArgs.Empty);
        }
    }
}
