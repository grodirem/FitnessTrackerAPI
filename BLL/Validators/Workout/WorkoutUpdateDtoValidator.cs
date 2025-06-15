using BLL.DTOs.Workout;
using FluentValidation;

namespace BLL.Validators.Workout;

public class WorkoutUpdateDtoValidator : BaseValidator<WorkoutUpdateDto>
{
    public WorkoutUpdateDtoValidator()
    {
        Include(new WorkoutCreateDtoValidator());
        RuleFor(x => x.Id).NotEmpty();
    }
}
