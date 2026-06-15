using Microsoft.AspNetCore.Mvc;
using TrustyTickets.Services;

namespace TrustyTickets.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private const string CookieName = "Session";
    private const int CookieUserIdKey = 1; // simple key for demo; real app would use proper session store

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.UserName) || string.IsNullOrEmpty(req.Password))
            return BadRequest(new { error = "UserName and Password required." });

        var (user, error) = await _auth.ValidateLoginAsync(req.UserName, req.Password, ct);
        if (user == null)
            return Unauthorized(new { error }); // Verbose: "User not found" vs "Invalid password"

        // Simple cookie "session": store user id in cookie (in production use proper session store)
        Response.Cookies.Append(CookieName, user.Id.ToString(), new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax, // Start with Lax for CSRF teaching
            Path = "/",
            MaxAge = TimeSpan.FromHours(2)
        });

        return Ok(new { id = user.Id, userName = user.UserName, displayName = user.DisplayName, isAdmin = user.IsAdmin });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(CookieName);
        return Ok();
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var val) || !int.TryParse(val, out var userId))
            return Unauthorized();

        var user = await _auth.GetUserByIdAsync(userId, ct);
        if (user == null) return Unauthorized();
        return Ok(new { id = user.Id, userName = user.UserName, displayName = user.DisplayName, isAdmin = user.IsAdmin });
    }
}

public record LoginRequest(string UserName, string Password);
