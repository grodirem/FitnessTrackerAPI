using BLL.DTOs.Integration;
using BLL.DTOs.Workout;

namespace BLL.Interfaces;

public interface IWorkoutService
{
    Task<WorkoutResponseDto> CreateWorkoutAsync(
        Guid userId, 
        WorkoutCreateDto createDto, 
        CancellationToken cancellationToken = default);
    
    Task<WorkoutResponseDto> GetWorkoutAsync(
        Guid workoutId, 
        Guid userId, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<WorkoutResponseDto>> GetUserWorkoutsAsync(
        Guid userId, 
        WorkoutFilterDto filterDto, 
        CancellationToken cancellationToken = default);
    
    Task UpdateWorkoutAsync(
        Guid workoutId, 
        Guid userId, 
        WorkoutUpdateDto updateDto, 
        CancellationToken cancellationToken = default);
   
    Task DeleteWorkoutAsync(
        Guid workoutId, 
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<WorkoutResponseDto> CreateWorkoutFromIntegrationAsync(
        Guid userId,
        ExternalWorkoutDto externalWorkoutDto,
        CancellationToken cancellationToken = default);
}
