namespace Common;

public class NotificationSettings
{
    public bool WorkoutReminders { get; set; } = true;
    public bool GoalProgressUpdates { get; set; } = true;
    public bool NewsAndAnnouncements { get; set; } = true;
    public string DailyNotificationTime { get; set; } = "19:00";
}
