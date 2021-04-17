using NoobSwarm.Hotkeys;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NoobSwarm.WPF.HotkeyVisualizer
{
    public class CommandNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return string.Empty;
            else if(value is IHotkeyCommand command)
                return command.GetType().Name + " - zukünftig Name des Hotkey Commands";
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
