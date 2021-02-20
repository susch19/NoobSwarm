using System.Collections.ObjectModel;

namespace NoobSwarm.WPF.Model
{
    public class MenuGroup
    {
        public string Name { get; set; }

        public ObservableCollection<MenuItem> Items { get; set; }
    }
}
