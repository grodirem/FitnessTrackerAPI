using BLL.DTOs.Goal;
using FluentValidation;

namespace BLL.Validators.Goal;

public class GoalSetDtoValidator : BaseValidator<GoalSetDto>
{
    public GoalSetDtoValidator()
    {
        RuleFor(x => x.TargetWorkoutsPerWeek).InclusiveBetween(1, 7);
        RuleFor(x => x.TargetCaloriesPerWorkout).InclusiveBetween(100, 1500);
    }
}
