using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.User;

public class NotificationSettingsDto
{
    public bool WorkoutReminders { get; set; } = true;

    public bool GoalProgressUpdates { get; set; } = true;

    public bool NewsAndAnnouncements { get; set; } = true;

    [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")]
    public string DailyNotificationTime { get; set; } = "19:00";
}
