using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Lights;

namespace NoobSwarm.Service.Windows.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LightController : ControllerBase
    {
        private readonly ILogger<LightController> logger;
        private readonly LightService lightService;

        public LightController(ILogger<LightController> logger)
        {
            this.logger = logger;
            lightService = TypeContainer.Get<LightService>();
        }

        [HttpPost]
        public ActionResult SetBrightness(byte brightness)
        {
            logger.LogDebug("Settings brightness to {brightness}", brightness);
            lightService.Brightness = brightness;

            return Ok();
        }
    }
}
