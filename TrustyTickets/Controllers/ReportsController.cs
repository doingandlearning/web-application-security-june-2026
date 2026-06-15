using Microsoft.AspNetCore.Mvc;
using TrustyTickets.Services;

namespace TrustyTickets.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ITicketService _tickets;
    private const string CookieName = "Session";

    public ReportsController(ITicketService tickets) => _tickets = tickets;

    private int? GetCurrentUserId()
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var val) || !int.TryParse(val, out var id)) return null;
        return id;
    }

    /// <summary>VULN (lab): SQL Injection — same bad pattern as ticket search.</summary>
    [HttpGet("assignees")]
    public async Task<IActionResult> Assignees([FromQuery] string? q, CancellationToken ct)
    {
        if (GetCurrentUserId() == null) return Unauthorized();
        var names = await _tickets.GetAssigneeNamesAsync(q, ct);
        return Ok(names);
    }
}
