using BLL.DTOs.Goal;
using FluentValidation;

namespace BLL.Validators.Goal;

public class GoalSetDtoValidator : BaseValidator<GoalSetDto>
{
    public GoalSetDtoValidator()
    {
        RuleFor(x => x.TargetWorkoutsPerWeek)
               .InclusiveBetween(1, 7)
               .WithMessage("Target workouts must be between 1 and 7 per week");

        RuleFor(x => x.TargetCaloriesPerWorkout)
            .InclusiveBetween(100, 1500)
            .WithMessage("Target calories must be between 100 and 1500 per workout");
    }
}
