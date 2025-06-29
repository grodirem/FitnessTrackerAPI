using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.User;

public class NotificationSettingsUpdateDto
{
    public bool? WorkoutReminders { get; set; }
    public bool? GoalProgressUpdates { get; set; }
    public bool? NewsAndAnnouncements { get; set; }

    [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")]
    public string? DailyNotificationTime { get; set; }
}
