using BLL.DTOs.User;
using FluentValidation;

namespace BLL.Validators.User;

public class UserRegisterDtoValidator : BaseValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BirthDate)
            .Must(BeValidDate)
            .WithMessage("Invalid birth date")
            .LessThan(DateTime.Today.AddYears(-12))
            .WithMessage("User must be at least 12 years old");
        RuleFor(x => x.Gender).IsInEnum();
    }
}
