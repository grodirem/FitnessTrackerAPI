using BLL.DTOs.Statistics;

namespace BLL.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsResponseDto> GetUserStatisticsAsync(Guid userId, StatisticsRequestDto requestDto);
    Task<StatisticsResponseDto> GetWorkoutTypeStatisticsAsync(Guid userId, StatisticsRequestDto requestDto);
}
