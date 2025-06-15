using Common.Enums;

namespace BLL.DTOs.Workout;

public class WorkoutFilterDto
{
    public WorkoutType? Type { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? MinDuration { get; set; }
    public int? MaxDuration { get; set; }
    public int? MinCalories { get; set; }
    public int? MaxCalories { get; set; }
    public string SortBy { get; set; } = "Date";
    public bool SortDescending { get; set; } = true;
}
