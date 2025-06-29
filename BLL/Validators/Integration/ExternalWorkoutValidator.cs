using BLL.DTOs.Integration;
using FluentValidation;

namespace BLL.Validators.Integration;

public class ExternalWorkoutValidator : AbstractValidator<ExternalWorkoutDto>
{
    public ExternalWorkoutValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Duration).GreaterThan(0);
        RuleFor(x => x.Calories).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Distance).GreaterThanOrEqualTo(0).When(x => x.Distance.HasValue);
        RuleFor(x => x.StartTime).LessThanOrEqualTo(x => x.EndTime);
        RuleFor(x => x.Source).IsInEnum();
    }
}
