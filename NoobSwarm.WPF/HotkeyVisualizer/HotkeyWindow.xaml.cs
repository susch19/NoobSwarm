using GalaSoft.MvvmLight;

using NoobSwarm.Hotkeys;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NoobSwarm.WPF.HotkeyVisualizer
{
    /// <summary>
    /// Interaction logic for HotkeyWindow.xaml
    /// </summary>
    public partial class HotkeyWindow : Window, INotifyPropertyChanged
    {
        private readonly Dispatcher dispatcher;
        private KeyNode currentNode;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<KeyNode> Nodes { get; } = new ObservableCollection<KeyNode>();

        public KeyNode CurrentNode
        {
            get => currentNode; private set
            {
                if (value == currentNode)
                    return;
                currentNode = value;
                NotifyPropertyChanged();
            }
        }

        public HotkeyWindow(HotKeyManager manager)
        {
            InitializeComponent();
            DataContext = this;
            dispatcher = Dispatcher.CurrentDispatcher;
            CurrentNode = manager.CurrentNode;
            Manager_NodeChanged(null, manager.CurrentNode);
            manager.NodeChanged += Manager_NodeChanged;
        }

        private void Manager_NodeChanged(object sender, KeyNode e)
        {
            CurrentNode = e;
            dispatcher.Invoke(() =>
            {
                Nodes.Clear();

                foreach (var item in e.Children.OrderBy(x => x.Key.ToString()))
                {
                    Nodes.Add(item.Value);
                }
                //Vulcan.NET.LedKey[] array = Enum.GetValues<Vulcan.NET.LedKey>();
                //for (int i = 0; i < array.Length - r; i++)
                //{
                //    Vulcan.NET.LedKey item = array[i];
                //    Nodes.Add(new KeyNode() { Key = item });
                //}
                Height = 80 + (Nodes.Count / 6) * 18;

                var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                Left = (screenWidth / 2) - (Width / 2);
                Top = (screenHeight - Height) * 0.95;
            });
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
