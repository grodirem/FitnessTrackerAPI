using Common.Enums;

namespace BLL.DTOs.Statistics;

public class WorkoutTypeCountResponseDto
{
    public WorkoutType Type { get; set; }
    public int TotalCount { get; set; }
}
