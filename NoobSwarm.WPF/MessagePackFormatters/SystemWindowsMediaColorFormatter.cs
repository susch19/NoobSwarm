using MessagePack;
using MessagePack.Formatters;
using System.Windows.Media;

namespace NoobSwarm.WPF.MessagePackFormatters
{
    public class SystemWindowsMediaColorFormatter : IMessagePackFormatter<Color>
    {
        public Color Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        public void Serialize(ref MessagePackWriter writer, Color value, MessagePackSerializerOptions options)
        {
            writer.WriteUInt8(value.A);
            writer.WriteUInt8(value.R);
            writer.WriteUInt8(value.G);
            writer.WriteUInt8(value.B);
        }
    }
}
