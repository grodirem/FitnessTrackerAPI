using BLL.DTOs.User;

namespace BLL.DTOs.Auth;

public class AuthResponseDto
{
    public bool IsAuthenticated { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpires { get; set; }
    public UserProfileDto UserProfile { get; set; }
    public string ErrorMessage { get; set; }
}