using FluentValidation;

namespace BLL.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected bool BeValidDate(DateTime date)
    {
        return date != DateTime.MinValue && date != DateTime.MaxValue;
    }
}
