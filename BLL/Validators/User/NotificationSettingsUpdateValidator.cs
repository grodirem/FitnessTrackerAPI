using BLL.DTOs.User;
using FluentValidation;

namespace BLL.Validators.User;

public class NotificationSettingsUpdateValidator : AbstractValidator<NotificationSettingsUpdateDto>
{
    public NotificationSettingsUpdateValidator()
    {
        RuleFor(x => x.DailyNotificationTime)
            .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")
            .When(x => !string.IsNullOrEmpty(x.DailyNotificationTime))
            .WithMessage("DailyNotificationTime must be in format 'HH:mm'");
    }
}
