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

        RuleFor(x => x.SortBy).Must(BeValidSortField)
            .WithMessage("Invalid sort field. Valid values: 'Type', 'Date', 'Duration', 'Calories'");
    }

    private bool BeValidSortField(string? sortBy)
    {
        if (string.IsNullOrEmpty(sortBy)) return true;
        var validFields = new[] { "Type", "Date", "Duration", "Calories" };
        return validFields.Contains(sortBy);
    }
}
