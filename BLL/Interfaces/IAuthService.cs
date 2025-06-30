using BLL.DTOs.Auth;
using BLL.DTOs.RefreshToken;
using BLL.DTOs.User;

namespace BLL.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto, CancellationToken cancellationToken = default);
    Task<RegistrationResponseDto> RegisterAsync(UserRegisterDto registerDto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
}
