using Common.Enums;

namespace BLL.DTOs.Statistics;

public class StatisticsResponseDto
{
    public int TotalWorkouts { get; set; }
    public int AverageWorkoutDuration { get; set; }
    public double? LongestDistance { get; set; }
    public WorkoutType MostFrequentWorkoutType { get; set; }
    public int MostCaloriesBurned { get; set; }
}
