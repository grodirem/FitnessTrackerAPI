using API.Extensions;
using BLL.DTOs.Statistics;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/statistics")]
public class StatisticsController : ControllerBase
{
    private IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var result = await _statisticsService.GetStatisticsAsync(User.GetUserId(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("workouts-count")]
    public async Task<IActionResult> GetWorkoutsCountAsync(
        [FromQuery] StatisticsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await _statisticsService.GetWorkoutsCountAsync(User.GetUserId(), request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("calories-dynamic")]
    public async Task<IActionResult> GetCaloriesDynamicAsync(
        [FromQuery] StatisticsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await _statisticsService.GetCaloriesDynamicAsync(User.GetUserId(), request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("workout-type")]
    public async Task<IActionResult> GetWorkoutTypeStatsAsync(
       [FromQuery] StatisticsRequestDto request,
       CancellationToken cancellationToken = default)
    {
        var result = await _statisticsService.GetWorkoutTypeStatsAsync(User.GetUserId(), request, cancellationToken);
        return Ok(result);
    }
}
