using BLL.DTOs.User;
using FluentValidation;

namespace BLL.Validators.User;

public class UserLoginDtoValidator : BaseValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
