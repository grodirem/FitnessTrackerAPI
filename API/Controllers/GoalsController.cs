using API.Extensions;
using BLL.DTOs.Goal;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/goals")]
[ApiController]
[Authorize]
public class GoalsController : Controller
{
    private IGoalService _goalService;

    public GoalsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    [HttpPost("set")]
    public async Task<IActionResult> SetGoalAsync(GoalSetDto goalSetDto, CancellationToken cancellationToken = default)
    {
        var result = await _goalService.SetGoalAsync(User.GetUserId(), goalSetDto, cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-current-goal")]
    public async Task<IActionResult> GetCurrentGoalAsync(CancellationToken cancellationToken)
    {
        var result = await _goalService.GetCurrentGoalAsync(User.GetUserId(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("deactivate-goal")]
    public async Task<IActionResult> DeactivateGoalAsync(
        Guid goalId, 
        CancellationToken cancellationToken)
    {
        await _goalService.DeactivateGoalAsync(goalId, User.GetUserId(), cancellationToken);
        return Ok();
    }
}
