using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Windows;
using NoobSwarm.WPF.Model;
using NoobSwarm.WPF.View;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Vulcan.NET;
using Key = System.Windows.Input.Key;

namespace NoobSwarm.WPF.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        #region Menu
        public ObservableCollection<MenuGroup> Menu { get; set; }

        public ICommand MenuClickedCommand { get; set; }
        #endregion

        public ICommand LoadedCommand { get; set; }
        public ICommand UnloadedCommand { get; set; }

        public ICommand MenuItemSettingsCommand { get; set; }
        public ICommand MenuItemInfoCommand { get; set; }
        public ICommand MenuItemExitCommand { get; set; }

        public ICommand KeyDownCommand { get; set; }
        
        public ISnackbarMessageQueue SnackbarMessageQueue { get; set; }
        public object CurrentView { get; set; }
        public string Title { get; set; }
        public ICommand TitleClickedCommand { get; set; }

        private readonly ObservableCollection<Exception> StartupExceptions = new();
        private readonly CockpitControl cockpitControl;
        private readonly CancellationTokenSource cts = new();
        
        public MainViewModel()
        {
            Title = "NoobSwarm";

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("logs\\log.txt")
                    .CreateLogger();

                // Code runs "for real"
                PropertyChanged += MainViewModel_PropertyChanged;

                LoadedCommand = new RelayCommand(Loaded);
                UnloadedCommand = new RelayCommand(Unloaded);
                KeyDownCommand = new RelayCommand<KeyEventArgs>(KeyDown);
                MenuClickedCommand = new RelayCommand<object>(MenuClicked);
                TitleClickedCommand = new RelayCommand(TitleClicked);

                MenuItemSettingsCommand = new RelayCommand(() => { });
                MenuItemInfoCommand = new RelayCommand(() => { /* TODO: Show dialog with version number of ui and packages dynamically */ });
                MenuItemExitCommand = new RelayCommand(Application.Current.Shutdown);

                cockpitControl = new CockpitControl();
                CurrentView = cockpitControl;

                Menu = new ObservableCollection<MenuGroup>()
                {
                    new MenuGroup()
                    {
                        Name = "Settings",
                        Items = new ObservableCollection<MenuItem>()
                        {
                            new MenuItem("Theme Designer", PackIconKind.ColorHelper, new ThemeDesignerControl()),
                            new MenuItem("Recording", PackIconKind.PlayBox, new RecordingControl()),
                            new MenuItem("Toolbar", PackIconKind.PlayBox, new ToolbarControl()),
                        }
                    }
                };

                var lightService = TypeContainer.Get<LightService>();
                lightService.AddToEnd(new HSVColorWanderEffect());
                lightService.Speed = 5;
                _ = Task.Run(() => lightService.UpdateLoop(cts.Token));

                var manager = TypeContainer.Get<HotKeyManager>();

                manager.Mode = HotKeyMode.Active;
                manager.HotKey = LedKey.FN_Key;

                var kb = TypeContainer.Get<VirtualHID.Keyboard>();
                LowLevelKeyboardHook hook = null;

                manager.StartedHotkeyMode += (s, e) => { hook = new LowLevelKeyboardHook(); hook.SetSupressKeyPress(); hook.HookKeyboard(); };
                manager.StoppedHotkeyMode += (s, e) => { hook?.Dispose(); hook = null; };

            }
        }

        private void KeyDown(KeyEventArgs e)
        {
            if (!IsControl())
                return;

            switch (e.Key)
            {
                case Key.S:
                    MenuItemSettingsCommand.Execute(null);
                    break;

                case Key.I:
                    MenuItemInfoCommand.Execute(null);
                    break;

                case Key.E:
                    MenuItemExitCommand.Execute(null);
                    break;
            }

            static bool IsControl()
            {
                return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            }
        }

        public void TitleClicked()
        {
            CurrentView = cockpitControl;
        }

        public void MenuClicked(object content)
        {
            CurrentView = content;
        }

        public void ShowError(Exception exception, [CallerMemberName] string caller = null)
        {
            Log.Error(exception, caller ?? "");
            if (SnackbarMessageQueue != null)
            {
                SnackbarMessageQueue.Enqueue($"error: {exception.Message}", "show more",
                    () => SnackbarMessageQueue.Enqueue($"{exception}"));
            }
            else
            {
                // if theres an exception at application startup, 
                // the viewmodel loads before the view and SnackbarMessageQueue is not declared yet
                StartupExceptions.Add(exception);
            }
        }

        private void ShowStartupExceptions()
        {
            foreach (Exception ex in StartupExceptions)
                ShowError(ex);

            StartupExceptions.Clear();
        }

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SnackbarMessageQueue):
                    ShowStartupExceptions();
                    break;
            }
        }

        private void Loaded()
        {

        }

        private void Unloaded()
        {
            cts.Cancel();
        }
    }
}
