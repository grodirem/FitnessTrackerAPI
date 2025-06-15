using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Workout;

public class WorkoutUpdateDto : WorkoutCreateDto
{
    [Required]
    public Guid Id { get; set; }
}