using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.WPF.ViewModel
{
    public class CockpitViewModel : ViewModelBase
    {
        public CockpitViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
            }
        }
    }
}
