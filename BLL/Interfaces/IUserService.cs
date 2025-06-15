using BLL.DTOs.User;

namespace BLL.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    Task UpdateUserProfileAsync(Guid userId, UserUpdateDto updateDto);
    Task DeleteAccountAsync(Guid userId);
}
