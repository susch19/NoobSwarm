
using Newtonsoft.Json;

using NonSucking.Framework.Extension.IoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.Serializations
{
    public class SerializationHelper
    {
        //public static void RegisterAllFormatters()
        //{
        //    var types = Assembly.GetExecutingAssembly().GetTypes();
        //    var formatters = new List<IMessagePackFormatter>();
        //    foreach (var t in types)
        //    {
        //        if (t.Namespace is null 
        //            || !t.Namespace.StartsWith("NoobSwarm")
        //            || t.IsAbstract
        //            || t.IsInterface
        //            || !t.IsAssignableTo(typeof(IMessagePackFormatter))
        //            || t.IsGenericType)
        //            continue;

        //        var instance = Activator.CreateInstance(t);
        //        if (instance is not null)
        //            formatters.Add((IMessagePackFormatter)instance);
        //    }

        //    MessagePack.Resolvers.StaticCompositeResolver.Instance.Register(formatters.ToArray());

        //    var compResolver = MessagePack.Resolvers.CompositeResolver.Create(
        //        MessagePack.Resolvers.StandardResolver.Instance,
        //        MessagePack.Resolvers.StaticCompositeResolver.Instance,
        //        MessagePack.Resolvers.TypelessContractlessStandardResolver.Instance);
        //    var defaultOptions = MessagePackSerializerOptions.Standard.WithResolver(compResolver);

        //    MessagePack.MessagePackSerializer.DefaultOptions = defaultOptions;
        //}

       
        public static readonly JsonSerializer TypeSafeSerializer = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,

        };
    }
}
