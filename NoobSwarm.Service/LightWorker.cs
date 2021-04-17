using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoobSwarm.Service
{
    public class LightWorker : BackgroundService
    {
        private readonly ILogger<LightWorker> _logger;
        private readonly LightService lightService;

        public LightWorker(ILogger<LightWorker> logger, Vulcan.NET.VulcanKeyboard keyboard)
        {
            _logger = logger;
            lightService = new LightService(keyboard);
            //lightService.AddToEnd(new HSVColorWanderEffect());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                lightService.UpdateLoop(stoppingToken);
            }
        }
    }

   
}
