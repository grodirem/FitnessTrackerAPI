namespace BLL.DTOs.Goal;

public class GoalResponseDto
{
    public Guid Id { get; set; }
    public int TargetWorkoutsPerWeek { get; set; }
    public int TargetCaloriesPerWorkout { get; set; }
    public bool Active { get; set; }
}
