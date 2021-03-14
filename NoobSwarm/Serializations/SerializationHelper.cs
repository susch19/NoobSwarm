
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
       
        public static readonly JsonSerializer TypeSafeSerializer = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,

        };


    }
}
