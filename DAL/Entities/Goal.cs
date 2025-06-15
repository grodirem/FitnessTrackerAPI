using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Goal
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Range(1, 7)]
    public int TargetWorkoutsPerWeek { get; set; }

    [Range(100, 1500)]
    public int TargetCaloriesPerWorkout { get; set; }

    public bool Active { get; set; } = true;


    public User User { get; set; }

}
