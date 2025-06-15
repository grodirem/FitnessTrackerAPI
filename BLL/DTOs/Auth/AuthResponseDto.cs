using BLL.DTOs.User;

namespace BLL.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpires { get; set; }
    public UserProfileDto UserProfile { get; set; }
}