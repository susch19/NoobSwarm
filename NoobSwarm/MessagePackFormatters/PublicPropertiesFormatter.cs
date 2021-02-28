using MessagePack;
using MessagePack.Formatters;
using System;
using System.Reflection;

namespace NoobSwarm.MessagePackFormatters
{
    public class PublicPropertiesFormatter<T> : IMessagePackFormatter<T>
    {
        private static PropertyInfo[] GetProperties() =>
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) ?? Array.Empty<PropertyInfo>();

        public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var c = Activator.CreateInstance<T>();

            foreach (var prop in GetProperties())
                prop.SetValue(c, MessagePackSerializer.Deserialize(prop.PropertyType, ref reader));

            return c;
        }

        public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
        {
            foreach (var prop in GetProperties())
                MessagePackSerializer.Serialize(prop.PropertyType, ref writer, prop.GetValue(value));
        }
    }
}
