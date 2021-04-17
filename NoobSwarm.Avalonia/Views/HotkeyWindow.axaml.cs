using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using NoobSwarm.Avalonia.ViewModels;

using System.Collections.ObjectModel;
using System.Linq;

namespace NoobSwarm.Avalonia.Views
{
    [ClassPropertyChangedAvalonia(typeof(HotkeyWindowViewModelProps))]
    public partial class HotkeyWindowViewModel : ViewModelBase
    {
        public ObservableCollection<KeyNode> Nodes { get; } = new ObservableCollection<KeyNode>();

        private HotKeyManager manager;

        public HotkeyWindowViewModel(HotKeyManager manager)
        {
            this.manager = manager;
        }

        private class HotkeyWindowViewModelProps
        {
            public KeyNode CurrentNode { get; set; }

        }
    }

    public partial class HotkeyWindow : Window
    {


        private KeyNode currentNode;
        private readonly HotkeyWindowViewModel cont;



        public HotkeyWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }
        public HotkeyWindow(HotKeyManager manager) : this()
        {
            cont = new HotkeyWindowViewModel(manager);
            cont.CurrentNode = manager.CurrentNode;
            DataContext = cont;
            Manager_NodeChanged(null, manager.CurrentNode);
            manager.NodeChanged += Manager_NodeChanged;

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Manager_NodeChanged(object sender, KeyNode e)
        {
            cont.CurrentNode = e;
            Dispatcher.UIThread.Post(() =>
            {
                cont.Nodes.Clear();

                foreach (var item in e.Children.OrderBy(x => x.Key.ToString()))
                {
                    cont.Nodes.Add(item.Value);
                }
                //Vulcan.NET.LedKey[] array = Enum.GetValues<Vulcan.NET.LedKey>();
                //for (int i = 0; i < array.Length - r; i++)
                //{
                //    Vulcan.NET.LedKey item = array[i];
                //    Nodes.Add(new KeyNode() { Key = item });
                //}
                Height = 80 + (cont.Nodes.Count / 6 * 18);

                var screenWidth = Screens.Primary.WorkingArea.Width;
                var screenHeight = Screens.Primary.WorkingArea.Height;
                Position = new((int)((screenWidth / 2) - (Width / 2)), (int)((screenHeight - Height) * 0.95));
            });
        }

    }
}
