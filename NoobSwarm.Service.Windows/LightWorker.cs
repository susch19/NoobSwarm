using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NoobSwarm.Service.Windows
{
    public class LightWorker : BackgroundService
    {
        private readonly ILogger<LightWorker> logger;
        private readonly LightService lightService;

        public LightWorker(ILogger<LightWorker> logger)
        {
            this.logger = logger;
            lightService = TypeContainer.Get<LightService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Needed otherwise application hangs
            await Task.Delay(1, stoppingToken);

            lightService.AddToEnd(new HSVColorWanderEffect());
            lightService.AddToEnd(new PressedCircleEffect(
                new InverseKeysColorEffect())
            {
                TriggerOnState = KeyChangeState.Pressed
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("LightService update loop running at: {time}", DateTimeOffset.Now);
                lightService.UpdateLoop(stoppingToken);
            }
        }
    }
}
