using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Goal;

public class GoalSetDto
{
    [Range(1, 7)]
    public int TargetWorkoutsPerWeek { get; set; }

    [Range(100, 1500)]
    public int TargetCaloriesPerWorkout { get; set; }

    public bool Active { get; set; } = true;
}