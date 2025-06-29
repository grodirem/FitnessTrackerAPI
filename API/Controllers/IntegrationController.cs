using API.Extensions;
using BLL.DTOs.Integration;
using BLL.Interfaces;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/integrations")]
    [Authorize]
    public class IntegrationController : ControllerBase
    {
        private readonly IIntegrationService _integrationService;

        public IntegrationController(IIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportWorkouts(
            IntegrationSourceType source,
            CancellationToken cancellationToken)
        {
            await _integrationService.ImportWorkoutsFromExternalServiceAsync(
                User.GetUserId(),
                source,
                cancellationToken);

            return Ok(new { Message = "Import started successfully" });
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncWorkouts(
            IntegrationSourceType source,
            CancellationToken cancellationToken)
        {
            await _integrationService.SyncWithExternalServiceAsync(
                User.GetUserId(),
                source,
                cancellationToken);

            return Ok(new { Message = "Sync completed successfully" });
        }

        [HttpGet("settings")]
        public async Task<ActionResult<IntegrationSettingsDto>> GetSettings()
        {
            var settings = await _integrationService.GetUserIntegrationSettingsAsync(User.GetUserId());
            return Ok(settings);
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] IntegrationSettingsDto settings)
        {
            await _integrationService.UpdateUserIntegrationSettingsAsync(User.GetUserId(), settings);
            return NoContent();
        }
    }
}
