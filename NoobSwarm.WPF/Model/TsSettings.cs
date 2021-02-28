using GalaSoft.MvvmLight;
using MessagePack;
using NoobSwarm.MessagePackFormatters;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Vulcan.NET;

namespace NoobSwarm.WPF.Model
{
    [MessagePackFormatter(typeof(PublicPropertiesFormatter<TsSettings>))]
    public class TsSettings : ObservableObject
    {
        public bool Enabled { get; set; }
        public string ApiKey { get; set; }
        public Color Color { get; set; }
        public ObservableCollection<LedKey> Keys { get; set; }
    }
}
