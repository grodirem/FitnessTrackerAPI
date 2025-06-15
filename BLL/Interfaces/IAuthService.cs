using BLL.DTOs.Auth;
using BLL.DTOs.User;

namespace BLL.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserRegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    Task RevokeTokenAsync(string refreshToken);
}
