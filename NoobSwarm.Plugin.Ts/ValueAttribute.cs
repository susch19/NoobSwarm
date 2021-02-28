using System;

namespace NoobSwarm.Plugin.Ts
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ValueAttribute : Attribute
    {
        public string Value { get; }

        public ValueAttribute(string value)
        {
            Value = value;
        }
    }
}
