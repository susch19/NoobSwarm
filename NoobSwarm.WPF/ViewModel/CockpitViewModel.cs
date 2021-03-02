using GalaSoft.MvvmLight;

using Microsoft.VisualBasic.Devices;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Lights;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.WPF.ViewModel
{
    public class CockpitViewModel : ViewModelBase
    {
        private LightService lightService;

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


                TypeContainer.Get<VulcanKeyboard>().VolumeKnobFxPressedReceived += VolumeKnobFxPressedReceived;
                lightService = TypeContainer.Get<LightService>();
            }
        }

        private void VolumeKnobFxPressedReceived(object sender, VolumeKnobFxArgs e)
        {
            if (VolumeKnobForBrighness)
                lightService.Brightness = e.Data < 2 ? (byte)0 : e.Data;
        }
    }
}
