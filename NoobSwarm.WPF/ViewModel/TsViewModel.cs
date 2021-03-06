using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessagePack;
using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Makros;
using NoobSwarm.Plugin.Ts;
using NoobSwarm.WPF.Model;
using NoobSwarm.WPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public class TsViewModel : ViewModelBase
    {
        public ICommand RecordingClearedCommand { get; set; }
        public ICommand RecordingStartedCommand { get; set; }
        public ICommand RecordingStoppedCommand { get; set; }
        public ICommand KeyRecordedCommand { get; set; }
        public ObservableCollection<MakroManager.RecordKey> RecordedKeys { get; set; }

        public TsSettings Settings { get; set; }

        private readonly LightService lightService;
        private readonly TsInfo tsInfo;
        private LightEffect effect;
        private const string save = "tssettings.save";

        private bool isTalking;

        public TsViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                lightService = TypeContainer.Get<LightService>();

                RecordingClearedCommand = new RelayCommand(() => Settings.Keys = new ObservableCollection<LedKey>());
                RecordingStartedCommand = new RelayCommand(() => Debug.WriteLine("recording started"));
                RecordingStoppedCommand = new RelayCommand<RecordKeysControl.RecordingStoppedEventArgs>(RecordingStopped);
                KeyRecordedCommand = new RelayCommand<RecordKeysControl.KeyRecordedEventArgs>(x => Debug.WriteLine("recorded key: " + x.RecordedKey));

                if (File.Exists(save))
                {
                    using var fs = File.OpenRead(save);
                    Settings = MessagePackSerializer.Deserialize<TsSettings>(fs);
                }
                else
                {
                    Settings = new TsSettings
                    {
                        Enabled = false,
                        ApiKey = "",
                        Color = Colors.Blue,
                        Keys = new(Enum.GetValues<LedKey>().Where(x => x >= LedKey.NUM_LOCK && x <= LedKey.NUM_ENTER))
                    };
                }
                RecordedKeys = new ObservableCollection<MakroManager.RecordKey>(Settings.Keys.Select(x => new MakroManager.RecordKey(LedKeyToKeyMapper.LedKeyToKey[x], 0, true)));
                effect = new SingleKeysColorEffect(
                    System.Drawing.Color.FromArgb(Settings.Color.A, Settings.Color.R, Settings.Color.G, Settings.Color.B), Settings.Keys);
                effect = new InverseKeysColorEffect(Settings.Keys);
                tsInfo = new TsInfo();
                tsInfo.TalkStatus += TsInfo_TalkStatus;

                if (!string.IsNullOrWhiteSpace(Settings.ApiKey) && Settings.Enabled)
                    Start();

                // Attach after we set all properties
                Settings.PropertyChanged += Settings_PropertyChanged;
            }
        }

        private void RecordingStopped(RecordKeysControl.RecordingStoppedEventArgs args)
        {
            Debug.WriteLine("recording stopped: " + string.Join(" ", args.RecordedKeys));
            var newKeys = args.RecordedKeys.Select(x => LedKeyToKeyMapper.KeyToLedKey[x.Key]).Distinct();

            if (!Settings.Keys.SequenceEqual(newKeys))
                Settings.Keys = new(newKeys);
        }

        private void TsInfo_TalkStatus(object sender, TalkStatusEventArgs e)
        {
            if (!e.IsMe)
                return;

            isTalking = e.IsTalking;
            UpdateTalkingEffect();
        }

        private void UpdateTalkingEffect()
        {
            if (isTalking)
                lightService.AddToEnd(effect);
            else
                lightService.RemoveLightEffect(effect);
        }

        private async Task Start()
        {
            await tsInfo.Connect(Settings.ApiKey);
            tsInfo.StartListen();
        }

        private async void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.Enabled):
                case nameof(Settings.ApiKey):
                    if (Settings.Enabled)
                    {
                        await Start();
                    }
                    else
                    {
                        tsInfo.StopListen();
                        tsInfo.Disconnect();
                    }
                    break;

                case nameof(Settings.Color):
                case nameof(Settings.Keys):
                    if (effect is SingleKeysColorEffect singleEffect)
                    {
                        var col = System.Drawing.Color.FromArgb(Settings.Color.A, Settings.Color.R, Settings.Color.G, Settings.Color.B);
                        singleEffect.KeyColors = Settings.Keys.ToDictionary(x => x, _ => col);
                    }
                    else if (effect is InverseKeysColorEffect inverseEffect)
                        inverseEffect.Keys = Settings.Keys.ToList();
                    break;
            }

            switch (e.PropertyName)
            {
                case nameof(Settings.Enabled):
                case nameof(Settings.ApiKey):
                case nameof(Settings.Color):
                case nameof(Settings.Keys):
                    using (var fs = File.OpenWrite(save))
                        MessagePackSerializer.Serialize(fs, Settings);
                    break;
            }
        }
    }
}
