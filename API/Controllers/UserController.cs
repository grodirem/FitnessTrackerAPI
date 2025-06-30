using API.Extensions;
using BLL.DTOs.User;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfileAsync()
    {
        var result = await _userService.GetUserProfileAsync(User.GetUserId());
        return Ok(result);
    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateUserProfileAsync(
        UserUpdateDto userUpdateDto, 
        CancellationToken cancellationToken)
    {
        await _userService.UpdateUserProfileAsync(User.GetUserId(), userUpdateDto, cancellationToken);
        return Ok();
    }

    [HttpPut("notification-settings")]
    public async Task<IActionResult> UpdateNotificationSettingsAsync(
        NotificationSettingsUpdateDto updateDto,
        CancellationToken cancellationToken)
    {
        await _userService.UpdateNotificationSettingsAsync(User.GetUserId(), updateDto, cancellationToken);
        return Ok();
    }

    [HttpDelete("delete-account")]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        await _userService.DeleteAccountAsync(User.GetUserId());
        return Ok();
    }
}
