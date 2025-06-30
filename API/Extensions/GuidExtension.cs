using BLL.Exceptions;
using System.Security.Claims;

namespace API.Extensions;

public static class GuidExtension
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var result))
        {
            throw new UnauthorizedException("User ID not found or invalid");
        }

        return result;
    }
}
