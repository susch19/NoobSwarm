using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Service
{
    public class Program
    {
        private static VulcanKeyboard keyboard;
        public static void Main(string[] args)
        {
            keyboard = VulcanKeyboard.Initialize();
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService(p =>new LightWorker(p.GetService<ILogger<LightWorker>>(), keyboard));
                    //services.AddHostedService<Worker2>();
                    //services.AddHostedService<Worker3>();
                });
    }
}
