using Common.Enums;

namespace BLL.DTOs.Statistics;

public class StatisticsResponseDto
{
    public int TotalWorkouts { get; set; }
    public int TotalCaloriesBurned { get; set; }
    public int AverageWorkoutDuration { get; set; }
    public WorkoutType FavoriteWorkoutType { get; set; }
    public Dictionary<string, int> WorkoutsByPeriod { get; set; }
    public Dictionary<WorkoutType, int> WorkoutsByType { get; set; }
}
