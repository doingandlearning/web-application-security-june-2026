using Microsoft.AspNetCore.Mvc;
using TrustyTickets.Services;

namespace TrustyTickets.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly ITicketService _tickets;
    private const string CookieName = "Session";

    public AttachmentsController(ITicketService tickets) => _tickets = tickets;

    private int? GetCurrentUserId()
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var val) || !int.TryParse(val, out var id)) return null;
        return id;
    }

    /// <summary>VULN (lab): IDOR — download any attachment by ID, no ownership check.</summary>
    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id, CancellationToken ct)
    {
        if (GetCurrentUserId() == null) return Unauthorized();
        var att = await _tickets.GetAttachmentByIdAsync(id, ct);
        if (att == null) return NotFound();
        return File(att.Content, att.ContentType, att.FileName);
    }
}
