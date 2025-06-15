namespace BLL.DTOs.Goal;

public class GoalDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int TargetWorkoutsPerWeek { get; set; }
    public int TargetCaloriesPerWorkout { get; set; }
    public bool Active { get; set; }
}