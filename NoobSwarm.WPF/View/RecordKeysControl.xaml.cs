using MaterialDesignThemes.Wpf;
using NoobSwarm.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoobSwarm.WPF.View
{
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

        public List<Makros.Key> RecordedKeys
        {
            get => (List<Makros.Key>)GetValue(RecordedKeysProperty);
            set => SetValue(RecordedKeysProperty, value);
        }

        public static readonly DependencyProperty RecordedKeysProperty =
            DependencyProperty.Register(nameof(RecordedKeys), typeof(List<Makros.Key>), typeof(RecordKeysControl), new PropertyMetadata(RecordedKeysChanged));

        private static void RecordedKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RecordKeysControl ctl)
                return;

            ctl.SetRecordedKeys((List<Makros.Key>)e.NewValue);
        }

        public class RecordingStoppedEventArgs : EventArgs
        {
            public ReadOnlyCollection<Makros.Key> RecordedKeys { get; set; }

            public RecordingStoppedEventArgs(ReadOnlyCollection<Makros.Key> recordedKeys)
            {
                RecordedKeys = recordedKeys;
            }
        }

        public class KeyRecordedEventArgs : EventArgs
        {
            public Makros.Key RecordedKey { get; set; }

            public KeyRecordedEventArgs(Makros.Key recordedKey)
            {
                RecordedKey = recordedKey;
            }
        }

        public event EventHandler RecordingStarted;
        public event EventHandler<RecordingStoppedEventArgs> RecordingStopped;
        public event EventHandler<KeyRecordedEventArgs> KeyRecorded;
        public event EventHandler RecordingCleared;

        private List<Makros.Key> recordedKeys = new();
        private LowLevelKeyboardHook hook;

        public RecordKeysControl()
        {
            InitializeComponent();
            Loaded += RecordKeysControl_Loaded;
        }

        private void RecordKeysControl_Loaded(object sender, RoutedEventArgs e)
        {
            HintAssist.SetHint(this, StartRecordingText);
        }

        private void SetRecordedKeys(List<Makros.Key> keys)
        {
            recordedKeys = keys;
            UpdateText(keys.ToArray());
        }

        private void UpdateText(params Makros.Key[] keys)
        {
            if (keys is null)
                return;

            Dispatcher.Invoke(() => KeyBox.Text += " " + string.Join(" ", keys.Select(x => x.ToString().Replace("VK_", ""))));
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
                hook = new LowLevelKeyboardHook();
                hook.OnKeyPressed += Hook_OnKeyPressed;
                hook.SetSupressKeyPress(BlockInput);
                hook.HookKeyboard();
                RecordingStarted?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Hook_OnKeyPressed(object sender, Makros.Key e)
        {
            recordedKeys.Add(e);
            UpdateText(new[] { e });
            Dispatcher.Invoke(() => KeyRecorded?.Invoke(this, new(e)));
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            HintAssist.SetHint(this, StartRecordingText);

            if (hook is not null)
            {
                hook.Dispose();
                hook = null;
                Dispatcher.Invoke(() => RecordingStopped?.Invoke(this, new(new(recordedKeys))));
            }
        }

        private void KeyBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(KeyBox.Text))
            {
                recordedKeys.Clear();
                RecordingCleared?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
