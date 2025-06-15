namespace BLL.DTOs.Auth;

public class RegistrationResponseDto
{
    public bool IsRegistered { get; set; }
    public IEnumerable<string> Errors { get; set; }
}
