using API.Extensions;
using BLL.DTOs.Workout;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/workouts")]
public class WorkoutsController : Controller
{
    private IWorkoutService _workoutService;

    public WorkoutsController(IWorkoutService workoutService)
    {
        _workoutService = workoutService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateWorkoutAsync(
        [FromBody] WorkoutCreateDto workoutCreateDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _workoutService.CreateWorkoutAsync(User.GetUserId(), workoutCreateDto, cancellationToken);
        return Ok(result);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetWorkoutAsync(
        Guid workoutId,
        CancellationToken cancellationToken = default)
    {
        var result = await _workoutService.GetWorkoutAsync(workoutId, User.GetUserId(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetUserWorkoutsAsync(
        [FromQuery] WorkoutFilterDto workoutFilterDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _workoutService.GetUserWorkoutsAsync(User.GetUserId(), workoutFilterDto, cancellationToken);
        return Ok(result);
    }

    [HttpPut("update-workout")]
    public async Task<IActionResult> UpdateWorkoutAsync(
        Guid workoutId,
        [FromBody] WorkoutUpdateDto updateDto,
        CancellationToken cancellationToken = default)
    {
        await _workoutService.UpdateWorkoutAsync(workoutId, User.GetUserId(), updateDto, cancellationToken);
        return Ok();
    }

    [HttpDelete("delete-workout")]
    public async Task<IActionResult> DeleteWorkoutAsync(
        Guid workoutId,
        CancellationToken cancellationToken = default)
    {
        await _workoutService.DeleteWorkoutAsync(workoutId, User.GetUserId(), cancellationToken);
        return Ok();
    }
}
