using Common.Enums;

namespace BLL.DTOs.Workout;

public class WorkoutResponseDto
{
    public Guid Id { get; set; }
    public WorkoutType Type { get; set; }
    public int Duration { get; set; }
    public int Calories { get; set; }
    public double? Distance { get; set; }
    public int? AverageHeartRate { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
    public Guid UserId { get; set; }
}
