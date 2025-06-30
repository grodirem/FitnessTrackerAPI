using BLL.DTOs.Workout;

namespace BLL.Validators.Workout;

public class WorkoutUpdateDtoValidator : BaseValidator<WorkoutUpdateDto>
{
    public WorkoutUpdateDtoValidator()
    {
        Include(new WorkoutCreateDtoValidator());
    }
}
