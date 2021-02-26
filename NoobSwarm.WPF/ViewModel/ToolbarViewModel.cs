using GalaSoft.MvvmLight;
using NoobSwarm.Windows;
using System.Collections.ObjectModel;

namespace NoobSwarm.WPF.ViewModel
{
    public class ToolbarViewModel : ViewModelBase
    {
        public ObservableCollection<ToolbarButtonInfo> ButtonInfos { get; set; }

        public ToolbarViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                ButtonInfos = new (Toolbar.GetToolbarButtonInfos(ToolbarButtonLocation.All));
            }
        }
    }
}
