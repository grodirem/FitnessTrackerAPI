using BLL.DTOs.Statistics;
using FluentValidation;

namespace BLL.Validators.Statistics;

public class StatisticsRequestDtoValidator : BaseValidator<StatisticsRequestDto>
{
    public StatisticsRequestDtoValidator()
    {
        When(x => x.StartDate.HasValue && x.EndDate.HasValue, () =>
        {
            RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        });
    }
}
