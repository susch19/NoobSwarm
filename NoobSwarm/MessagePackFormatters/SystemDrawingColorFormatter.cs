using MessagePack;
using MessagePack.Formatters;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.MessagePackFormatters
{
    public class SystemDrawingColorFormatter : IMessagePackFormatter<Color>
    {
        public Color Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return Color.FromArgb(reader.ReadInt32());
        }

        public void Serialize(ref MessagePackWriter writer, Color value, MessagePackSerializerOptions options)
        {
            writer.WriteInt32(value.ToArgb());
        }
    }
}
