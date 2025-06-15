using BLL.DTOs.User;
using FluentValidation;

namespace BLL.Validators.User;

public class UserUpdateDtoValidator : BaseValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.Name).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Name));
        RuleFor(x => x.BirthDate)
            .Must(BeValidDate)
            .WithMessage("Invalid birth date")
            .LessThan(DateTime.Today.AddYears(-12))
            .WithMessage("User must be at least 12 years old");
    }
}
