using BLL.DTOs.Workout;
using FluentValidation;

namespace BLL.Validators.Workout;

public class WorkoutFilterDtoValidator : BaseValidator<WorkoutFilterDto>
{
    public WorkoutFilterDtoValidator()
    {
        When(x => x.FromDate.HasValue && x.ToDate.HasValue, () =>
        {
            RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate);
        });
    }
}
