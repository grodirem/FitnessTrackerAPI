using BLL.DTOs.Workout;
using FluentValidation;

namespace BLL.Validators.Workout;

public class WorkoutCreateDtoValidator : BaseValidator<WorkoutCreateDto>
{
    public WorkoutCreateDtoValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid workout type. Valid values: 'Running', 'Swimming', 'Cycling'");
        RuleFor(x => x.Duration).InclusiveBetween(1, 1000);
        RuleFor(x => x.Calories).InclusiveBetween(0, 10000);
        RuleFor(x => x.Distance).InclusiveBetween(0, 1000).When(x => x.Distance.HasValue);
        RuleFor(x => x.Date).Must(BeValidDate).WithMessage("Invalid date");
    }
}
