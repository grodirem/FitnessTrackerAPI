using API.Extensions;
using BLL.DTOs.Goal;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/goals")]
[ApiController]
[Authorize]
public class GoalsController : ControllerBase
{
    private IGoalService _goalService;

    public GoalsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    [HttpPost]
    public async Task<IActionResult> SetGoalAsync(GoalSetDto goalSetDto, CancellationToken cancellationToken = default)
    {
        var result = await _goalService.SetGoalAsync(User.GetUserId(), goalSetDto, cancellationToken);
        return Ok(result);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentGoalAsync(CancellationToken cancellationToken)
    {
        var result = await _goalService.GetCurrentGoalAsync(User.GetUserId(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{goalId:guid}/deactivate")]
    public async Task<IActionResult> DeactivateGoalAsync(
        [FromRoute] Guid goalId, 
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _goalService.DeactivateGoalAsync(goalId, userId, cancellationToken);
        return Ok();
    }
}
