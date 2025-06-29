using BLL.DTOs.Statistics;

namespace BLL.Interfaces;

public interface IStatisticsService
{
    Task<int> GetWorkoutsCountAsync(
       Guid userId,
       StatisticsRequestDto request,
       CancellationToken cancellationToken = default);

    Task<StatisticsResponseDto> GetStatisticsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<CaloriesDynamicResponseDto> GetCaloriesDynamicAsync(
        Guid userId,
        StatisticsRequestDto request,
        CancellationToken cancellationToken = default);

    Task<List<WorkoutTypeCountResponseDto>> GetWorkoutTypeStatsAsync(
       Guid userId,
       StatisticsRequestDto request,
       CancellationToken cancellationToken = default);
}
