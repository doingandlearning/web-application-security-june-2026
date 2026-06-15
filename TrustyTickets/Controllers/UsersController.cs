using Microsoft.AspNetCore.Mvc;
using TrustyTickets.Services;

namespace TrustyTickets.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _auth;
    private const string CookieName = "Session";

    public UsersController(IAuthService auth) => _auth = auth;

    private async Task<int?> GetCurrentUserIdAsync(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var val) || !int.TryParse(val, out var id)) return null;
        var user = await _auth.GetUserByIdAsync(id, ct);
        return user?.Id;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        if (await GetCurrentUserIdAsync(ct) == null) return Unauthorized();
        var users = await _auth.GetAllUsersAsync(ct);
        return Ok(users.Select(u => new { u.Id, u.UserName, u.DisplayName }));
    }
}
