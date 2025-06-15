using BLL.DTOs.Workout;

namespace BLL.Interfaces;

public interface IWorkoutService
{
    Task<WorkoutResponseDto> CreateWorkoutAsync(Guid userId, WorkoutCreateDto createDto);
    Task<WorkoutResponseDto> GetWorkoutAsync(Guid workoutId, Guid userId);
    Task<IEnumerable<WorkoutResponseDto>> GetUserWorkoutsAsync(Guid userId, WorkoutFilterDto filterDto);
    Task UpdateWorkoutAsync(Guid workoutId, Guid userId, WorkoutUpdateDto updateDto);
    Task DeleteWorkoutAsync(Guid workoutId, Guid userId);
}
