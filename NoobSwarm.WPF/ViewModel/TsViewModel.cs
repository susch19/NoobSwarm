using GalaSoft.MvvmLight;
using MessagePack;
using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Plugin.Ts;
using NoobSwarm.WPF.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public class TsViewModel : ViewModelBase
    {
        public TsSettings Settings { get; set; }

        private readonly LightService lightService;
        private readonly TsInfo tsInfo;
        private SingleKeysColorEffect effect;
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

                effect = new SingleKeysColorEffect(
                    System.Drawing.Color.FromArgb(Settings.Color.A, Settings.Color.R, Settings.Color.G, Settings.Color.B), Settings.Keys);

                tsInfo = new TsInfo();
                tsInfo.TalkStatus += TsInfo_TalkStatus;

                if (!string.IsNullOrWhiteSpace(Settings.ApiKey) && Settings.Enabled)
                    Start();

                // Attach after we set all properties
                Settings.PropertyChanged += Settings_PropertyChanged;
            }
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
                    var oldEffect = effect;
                    effect = new SingleKeysColorEffect(System.Drawing.Color.FromArgb(Settings.Color.A, Settings.Color.R, Settings.Color.G, Settings.Color.B), Settings.Keys);
                    lightService.RemoveLightEffect(oldEffect);
                    UpdateTalkingEffect();
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
