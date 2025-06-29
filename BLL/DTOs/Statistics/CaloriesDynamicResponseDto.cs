namespace BLL.DTOs.Statistics;

public class CaloriesDynamicResponseDto
{
    public List<CaloriesDynamicDto> DailyCalories { get; set; } = [];
    public int TotalCalories { get; set; }
    public double AverageCaloriesPerDay { get; set; }
}
