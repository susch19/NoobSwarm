using MaterialDesignThemes.Wpf;

namespace NoobSwarm.WPF.Model
{
    public class MenuItem
    {
        public string Name { get; set; }

        public PackIconKind Icon { get; set; }

        public object Content { get; set; }

        public MenuItem(string name, PackIconKind icon, object content)
        {
            Name = name;
            Icon = icon;
            Content = content;
        }
    }
}
