using GalaSoft.MvvmLight;

using Microsoft.VisualBasic.Devices;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Lights.LightEffects.Duration;
using NoobSwarm.Lights.LightEffects.Wrapper;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public class CockpitViewModel : ViewModelBase
    {
        private LightService lightService;
        private LightEffectWrapper lightEffect;


        public bool VolumeKnobForBrighness { get; set; }

        public CockpitViewModel()
        {
            VolumeKnobForBrighness = true;
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {

                var kb = TypeContainer.Get<IVulcanKeyboard>();
                kb.VolumeKnobFxPressedReceived += VolumeKnobFxPressedReceived;
                kb.DPITurnedReceived += Kb_DPITurnedReceived;
                kb.VolumeKnobPressedReceived += Kb_VolumeKnobPressedReceived;

                lightService = TypeContainer.Get<LightService>();
                lightEffect = new TimeSpanDurationLightEffectWrapper(new SolidColorEffect(System.Drawing.Color.White), TimeSpan.FromSeconds(2));
                //lightService.AddToEnd(lightEffect);
            }
        }

        private void Kb_VolumeKnobPressedReceived(object sender, VolumeKnobArgs e)
        {
            if (e.IsPressed)
                lightService.Speed++;
            else
                lightService.Speed--;
            //lightEffect.Active = true;
        }

        private void Kb_DPITurnedReceived(object sender, VolumeKnDirectionArgs e)
        {
            if (e.TurnedRight)
                lightService.Speed++;
            else
                lightService.Speed--;
            //lightEffect.Active = true;
        }

        private void VolumeKnobFxPressedReceived(object sender, VolumeKnobFxArgs e)
        {
            if (VolumeKnobForBrighness)
            {
                var ll = lightService.LightLayers.FirstOrDefault();

                ll.MainEffect.Brightness = (byte)Math.Min(255, (255 / 68) * ( e.Data < 2 ? (byte)0 : e.Data));

            }
        }
    }
}
