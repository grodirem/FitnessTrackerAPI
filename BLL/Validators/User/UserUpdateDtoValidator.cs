using BLL.DTOs.User;
using FluentValidation;

namespace BLL.Validators.User;

public class UserUpdateDtoValidator : BaseValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.BirthDate)
            .Must(BeValidDate).WithMessage("Invalid birth date")
            .LessThan(DateTime.Today.AddYears(-12)).WithMessage("User must be at least 12 years old")
            .When(x => x.BirthDate != default);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value")
            .When(x => x.Gender.HasValue);
    }
}
