using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using MaterialDesignThemes.Wpf;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Lights.LightEffects.Wrapper;
using NoobSwarm.WPF.HotkeyVisualizer;
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
using System.Windows.Threading;

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
        public ICommand MenuItemReloadKeyboardCommand { get; set; }
        public ICommand MenuItemLightEffectCommand { get; set; }
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

        private readonly LightService lightService;
        private readonly HotKeyManager manager;


        private Dispatcher dispatcher;
        private HotkeyWindow hotkeyWindow;

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

                dispatcher = Dispatcher.CurrentDispatcher;

                // Code runs "for real"
                PropertyChanged += MainViewModel_PropertyChanged;

                InitializeCommands();

                cockpitControl = new CockpitControl();
                CurrentView = cockpitControl;
                CreateMenu();

                lightService = TypeContainer.Get<LightService>();

                if (lightService.LightLayers.Count == 0)
                {
                    lightService.AddToStart(new LightEffectWrapper(new HSVColorWanderEffect()));

                    //lightService.AddToStart(new LightEffectWrapper(new SolidColorEffect(Color.Black)));
                    //                    lightService.AddToEnd(new ColorizeLightEffectWrapper(new PressedCircleEffect(),
                    //                        new ColorizeLightEffectWrapper(new BreathingColorEffect(),
                    //new HSVColorGradientCycleEffect())));

                }
                else
                {
                    //lightService.AddToStart(new LightEffectWrapper(new SolidColorEffect(Color.Black)));

                    lightService.AddToStart(new LightEffectWrapper(new HSVColorWanderEffect()));

                }
                lightService.Speed = 5;
                _ = Task.Run(() => lightService.UpdateLoop(cts.Token));

                manager = TypeContainer.Get<HotKeyManager>();

                manager.Mode = HotKeyMode.Active;

                var kb = TypeContainer.Get<VirtualHID.Keyboard>();
                var vkb = TypeContainer.Get<IVulcanKeyboard>();
                MenuItemReloadKeyboardCommand = new RelayCommand(() => { vkb.Disconnect(); vkb.Connect(); });
                MenuItemLightEffectCommand = new RelayCommand(() => { lightService.Serialize(); });
                Startup();
            }
        }

        private void InitializeCommands()
        {
            LoadedCommand = new RelayCommand(Loaded);
            UnloadedCommand = new RelayCommand(Unloaded);
            KeyDownCommand = new RelayCommand<KeyEventArgs>(KeyDown);
            MenuClickedCommand = new RelayCommand<object>(MenuClicked);
            TitleClickedCommand = new RelayCommand(TitleClicked);

            MenuItemSettingsCommand = new RelayCommand(() => { });

            MenuItemInfoCommand = new RelayCommand(() => { /* TODO: Show dialog with version number of ui and packages dynamically */ });
            MenuItemExitCommand = new RelayCommand(Application.Current.Shutdown);
        }

        private void CreateMenu()
        {
            Menu = new ObservableCollection<MenuGroup>()
                {
                    new MenuGroup()
                    {
                        Name = "Settings",
                        Items = new ObservableCollection<MenuItem>()
                        {
                            new MenuItem("Theme Designer", PackIconKind.ColorHelper, new ThemeDesignerControl()),
                            //new MenuItem("Recording", PackIconKind.PlayBox, new RecordingControl()),
                            new MenuItem("Recording", PackIconKind.PlayBox, new RecordingControl()),
                            new MenuItem("Playback", PackIconKind.PlayBox, new PlaybackControl()),
                            new MenuItem("Toolbar", PackIconKind.PlayBox, new ToolbarControl()),
                            new MenuItem("TeamSpeak", PackIconKind.VoiceChat, new TsControl()),
                            new MenuItem("Makro Overview", PackIconKind.Delete, new MakroOverview()),
                        }
                    }
                };
        }

        private void Startup()
        {
            manager.StartedHotkeyMode += Hkm_StartedHotkeyMode;
            manager.StoppedHotkeyMode += Hkm_StoppedHotkeyMode;
        }

        private void Hkm_StartedHotkeyMode(object sender, EventArgs e)
        {
            bool flag = hotkeyWindow == null;
            if (flag)
            {
                dispatcher.Invoke(delegate ()
                {
                    hotkeyWindow = new HotkeyWindow(manager);
                    hotkeyWindow.Show();
                });
            }
        }

        private void Hkm_StoppedHotkeyMode(object sender, EventArgs e)
        {
            bool flag = hotkeyWindow != null;
            if (flag)
            {
                dispatcher.Invoke(delegate ()
                {
                    HotkeyWindow hotkeyWindow = this.hotkeyWindow;
                    if (hotkeyWindow != null)
                    {
                        hotkeyWindow.Close();
                    }
                    this.hotkeyWindow = null;
                });
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
