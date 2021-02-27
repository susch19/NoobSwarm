using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NoobSwarm.WPF.View
{
    /// <summary>
    /// Interaktionslogik für RecordingControl.xaml
    /// </summary>
    public partial class RecordingControl : UserControl
    {
        public RecordingControl()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                ((TextBox)sender).Focus();
        }

        private void Textbox_ScrollToEnd(object sender, TextChangedEventArgs e)
        {
            if(sender is TextBox tb)
            {
                tb.CaretIndex = tb.Text.Length;
                tb.ScrollToEnd();
            }
        }
    }
}
