﻿using BLL.DTOs.Goal;
using Common.Enums;

namespace BLL.DTOs.User;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public DateTime CreatedAt { get; set; }

    public NotificationSettingsDto NotificationSettings { get; set; } = new();
    public ICollection<GoalProfileDto> Goals { get; set; } = [];
}