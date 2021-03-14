using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Lights;
using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using Vulcan.NET;

namespace NoobSwarm.Service.Windows
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(Constants.LogDirectory.FullName, "NoobSwarm.Service.Windows.log"), rollOnFileSizeLimit: true)
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                TypeContainer.Register<MakroManager>(InstanceBehaviour.Singleton);
                TypeContainer.Register<Keyboard, Keyboard>(InstanceBehaviour.Singleton);
                TypeContainer.Register<IKeyboard, Keyboard>(InstanceBehaviour.Singleton);
                TypeContainer.Register(VulcanKeyboard.Initialize());

                var service = LightService.Deserialize(Constants.LightServiceSettings);
                TypeContainer.Register(service);

                var hkm = HotKeyManager.Deserialize(Constants.HotKeyManagerSettings);
                TypeContainer.Register(hkm);

                SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

                CreateHostBuilder(args)
                    .Build()
                    .Run();

                TypeContainer.Get<ITypeContainer>().Dispose();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            var vulcanKeyboard = TypeContainer.Get<VulcanKeyboard>();

            if (e.Reason == SessionSwitchReason.SessionUnlock)
                vulcanKeyboard.Connect();
            else if (e.Reason == SessionSwitchReason.SessionLock)
                vulcanKeyboard.Disconnect();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<LightWorker>();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://localhost:5010", "https://localhost:5011");
            })
            .UseSerilog();
    }
}
