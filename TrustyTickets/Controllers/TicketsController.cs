using Microsoft.AspNetCore.Mvc;
using TrustyTickets.Services;

namespace TrustyTickets.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _tickets;
    private readonly IAuthService _auth;
    private const string CookieName = "Session";

    public TicketsController(ITicketService tickets, IAuthService auth)
    {
        _tickets = tickets;
        _auth = auth;
    }

    private async Task<int?> GetCurrentUserIdAsync(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var val) || !int.TryParse(val, out var id)) return null;
        var user = await _auth.GetUserByIdAsync(id, ct);
        return user?.Id;
    }

    /// <summary>VULN (teaching): SQLi via search. Admin sees all, user sees own; GetById has no ownership check.</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? search, CancellationToken ct)
    {
        var userId = await GetCurrentUserIdAsync(ct);
        if (userId == null) return Unauthorized();
        var user = await _auth.GetUserByIdAsync(userId.Value, ct);
        var isAdmin = user?.IsAdmin ?? false;
        var list = await _tickets.GetTicketsForUserAsync(userId.Value, isAdmin, search, ct);
        return Ok(list);
    }

    /// <summary>VULN (teaching): IDOR — no ownership check; returns any ticket by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        if (await GetCurrentUserIdAsync(ct) == null) return Unauthorized();
        var ticket = await _tickets.GetTicketByIdAsync(id, ct);
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    /// <summary>CSRF (teaching): state-changing, no anti-forgery.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest req, CancellationToken ct)
    {
        var userId = await GetCurrentUserIdAsync(ct);
        if (userId == null) return Unauthorized();
        var ticket = await _tickets.CreateTicketAsync(userId.Value, req.Title, req.Description ?? "", ct);
        return CreatedAtAction(nameof(GetById), new { id = ticket!.Id }, ticket);
    }

    /// <summary>CSRF (lab): state-changing, no anti-forgery.</summary>
    [HttpPost("{id:int}/assign")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignRequest req, CancellationToken ct)
    {
        if (await GetCurrentUserIdAsync(ct) == null) return Unauthorized();
        var ok = await _tickets.AssignTicketAsync(id, req.AssigneeId, ct);
        if (!ok) return NotFound();
        return Ok();
    }

    /// <summary>Comments stored and rendered with innerHTML — XSS teaching.</summary>
    [HttpPost("{id:int}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentRequest req, CancellationToken ct)
    {
        var userId = await GetCurrentUserIdAsync(ct);
        if (userId == null) return Unauthorized();
        var comment = await _tickets.AddCommentAsync(id, userId.Value, req.Body ?? "", ct);
        if (comment == null) return NotFound();
        return Ok(comment);
    }
}

public record CreateTicketRequest(string Title, string? Description);
public record AssignRequest(int AssigneeId);
public record AddCommentRequest(string? Body);
