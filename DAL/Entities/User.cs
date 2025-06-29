using Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public string NotificationSettingsJson { get; set; }
        = "{\"WorkoutReminders\":true," +
        "\"GoalProgressUpdates\":true," +
        "\"NewsAndAnnouncements\":true," +
        "\"DailyNotificationTime\":\"19:00\"}";

    public string? IntegrationSettingsJson { get; set; }

    public ICollection<Workout> Workouts { get; set; }
    public ICollection<Goal> Goals { get; set; }
}
