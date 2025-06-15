using BLL.DTOs.User;
using FluentValidation;

namespace BLL.Validators.User;

public class UserRegisterDtoValidator : BaseValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birth date is required")
            .Must(BeValidDate).WithMessage("Invalid birth date")
            .LessThan(DateTime.Today.AddYears(-12)).WithMessage("User must be at least 12 years old");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value");
    }
}