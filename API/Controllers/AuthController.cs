using API.Extensions;
using BLL.DTOs.RefreshToken;
using BLL.DTOs.User;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto, CancellationToken cancellationToken = default)
    {
        var result = await _authService.LoginAsync(userLoginDto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto, CancellationToken cancellationToken = default)
    {
        var result = await _authService.RegisterAsync(userRegisterDto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenRequest, cancellationToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        await _authService.LogoutAsync(User.GetUserId(), cancellationToken);
        return Ok();
    }
}
