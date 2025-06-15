using Common.Enums;

namespace BLL.DTOs.Integration;

public class ExternalWorkoutDto
{
    public WorkoutType Type { get; set; }
    public int Duration { get; set; }
    public int Calories { get; set; }
    public double? Distance { get; set; }
    public DateTime Date { get; set; }
    public IntegrationSourceType Source { get; set; }
}
