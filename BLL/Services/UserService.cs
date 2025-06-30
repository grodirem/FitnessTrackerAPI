using AutoMapper;
using BLL.DTOs.User;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ValidationException = BLL.Exceptions.ValidationException;

namespace BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IValidator<UserUpdateDto> _updateValidator;
    private readonly IValidator<NotificationSettingsUpdateDto> _notificationValidator;


    public UserService(
        UserManager<User> userManager,
        IMapper mapper,
        IValidator<UserUpdateDto> updateValidator,
        IValidator<NotificationSettingsUpdateDto> notificationValidator)
    {
        _userManager = userManager;
        _mapper = mapper;
        _updateValidator = updateValidator;
        _notificationValidator = notificationValidator;
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var user = await _userManager.Users
            .Include(u => u.Goals)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task UpdateUserProfileAsync(
        Guid userId, 
        UserUpdateDto updateDto, 
        CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(updateDto, cancellationToken);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (!string.IsNullOrEmpty(updateDto.Name))
        {
            user.Name = updateDto.Name;
        }

        if (updateDto.BirthDate != default)
        {
            user.BirthDate = updateDto.BirthDate;
        }

        if (updateDto.Gender.HasValue)
        {
            user.Gender = updateDto.Gender.Value;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to update user profile");
        }
    }

    public async Task UpdateNotificationSettingsAsync(
        Guid userId,
        NotificationSettingsUpdateDto updateDto,
        CancellationToken cancellationToken = default)
    {
        await _notificationValidator.ValidateAndThrowAsync(updateDto, cancellationToken);

        var user = await GetUserWithNotificationsAsync(userId, cancellationToken);
        var notificationSettings = DeserializeUserSettings(user.NotificationSettingsJson);

        UpdateNotificationSettings(notificationSettings, updateDto);

        await SaveUpdatedSettingsAsync(user, notificationSettings);
    }

    private async Task<User> GetUserWithNotificationsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user ?? throw new NotFoundException("User not found");
    }

    private NotificationSettingsDto DeserializeUserSettings(string settingsJson)
    {
        return string.IsNullOrWhiteSpace(settingsJson)
            ? new NotificationSettingsDto()
            : JsonSerializer.Deserialize<NotificationSettingsDto>(
                settingsJson,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    private void UpdateNotificationSettings(
        NotificationSettingsDto settings,
        NotificationSettingsUpdateDto updateDto)
    {
        if (updateDto.WorkoutReminders.HasValue)
            settings.WorkoutReminders = updateDto.WorkoutReminders.Value;

        if (updateDto.GoalProgressUpdates.HasValue)
            settings.GoalProgressUpdates = updateDto.GoalProgressUpdates.Value;

        if (updateDto.NewsAndAnnouncements.HasValue)
            settings.NewsAndAnnouncements = updateDto.NewsAndAnnouncements.Value;

        if (!string.IsNullOrWhiteSpace(updateDto.DailyNotificationTime))
            settings.DailyNotificationTime = updateDto.DailyNotificationTime;
    }

    private async Task SaveUpdatedSettingsAsync(User user, NotificationSettingsDto settings)
    {
        user.NotificationSettingsJson = JsonSerializer.Serialize(
            settings,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new Exception("Failed to update notification settings");
    }

    public async Task DeleteAccountAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            throw new Exception("Failed to clear user refresh tokens");
        }

        var deleteResult = await _userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
        {
            throw new Exception("Failed to delete user account");
        }
    }
}