using BLL.DTOs.User;

namespace BLL.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    
    Task UpdateUserProfileAsync(
        Guid userId, 
        UserUpdateDto updateDto, 
        CancellationToken cancellationToken = default);

    Task UpdateNotificationSettingsAsync(
        Guid userId,
        NotificationSettingsUpdateDto updateDto,
        CancellationToken cancellationToken = default);

    Task DeleteAccountAsync(Guid userId);
}
