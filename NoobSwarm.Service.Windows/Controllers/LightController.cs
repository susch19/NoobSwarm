using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Lights;
using System.ComponentModel.DataAnnotations;

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

        [HttpGet("Brightness")]
        public ActionResult<byte> Brightness()
        {
            logger.LogDebug("Requesting brightness");
            return lightService.Brightness;
        }


        [HttpPost("Brightness")]
        public ActionResult<byte> Brightness([Range(0, 69)]byte brightness)
        {
            logger.LogDebug("Settings brightness to {brightness}", brightness);
            lightService.Brightness = brightness;

            return lightService.Brightness;
        }

        [HttpPost("Speed")]
        public ActionResult<ushort> Speed(ushort speed)
        {
            logger.LogDebug("Settings speed to {speed}", speed);
            lightService.Speed = speed;

            return lightService.Speed;
        }

        [HttpGet("Speed")]
        public ActionResult<ushort> Speed()
        {
            logger.LogDebug("Requesting speed");
            return lightService.Speed;
        }

        [HttpPost("Save")]
        public ActionResult Save()
        {
            logger.LogDebug("Saving");
            lightService.Serialize(Constants.LightServiceSettings);

            return Ok();
        }
    }
}
