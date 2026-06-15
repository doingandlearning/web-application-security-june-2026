using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustyTickets.Ai;
using TrustyTickets.Data;
using TrustyTickets.Models;
using TrustyTickets.Services;

namespace TrustyTickets.Controllers;

[ApiController]
[Route("api/ai/comment-summaries")]
public class AiCommentSummariesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuthService _auth;
    private const string CookieName = "Session";

    public AiCommentSummariesController(AppDbContext db, IAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    private async Task<User?> GetCurrentUserAsync(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var val) || !int.TryParse(val, out var id))
            return null;

        return await _auth.GetUserByIdAsync(id, ct);
    }

    [HttpPost("enhance")]
    public async Task<IActionResult> Enhance([FromBody] CommentSummaryRequest req, CancellationToken ct)
    {
        var user = await GetCurrentUserAsync(ct);
        if (user == null) return Unauthorized();

        if (req.TicketId <= 0) return BadRequest(new { error = "ticketId required" });
        if (req.CommentIds == null) return BadRequest(new { error = "commentIds required" });
        if (req.CommentIds.Length == 0) return BadRequest(new { error = "commentIds must not be empty" });

        SummarizationMode mode = req.Mode == "safe" ? SummarizationMode.Safe : SummarizationMode.Unsafe;

        var ticket = await _db.Tickets
            .Include(t => t.Owner)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == req.TicketId, ct);

        if (ticket == null) return NotFound(new { error = "ticket not found" });

        var canAccess =
            user.IsAdmin ||
            ticket.OwnerId == user.Id ||
            (ticket.AssignedToId.HasValue && ticket.AssignedToId.Value == user.Id);

        if (!canAccess) return Forbid();

        var comments = await _db.Comments
            .Include(c => c.Author)
            .Where(c => c.TicketId == req.TicketId && req.CommentIds.Contains(c.Id))
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(ct);

        if (comments.Count != req.CommentIds.Length)
            return BadRequest(new { error = "one or more commentIds are invalid for this ticket" });

        var summarizer = new PromptInjectionCommentSummarizer();
        var result = await summarizer.SummarizeAsync(comments, mode, ct);

        return Ok(new
        {
            enhancedSummary = result.EnhancedSummary,
            promptInjectionDetected = result.PromptInjectionDetected,
            leakedSystemPrompt = result.LeakedSystemPrompt,
            mode = req.Mode
        });
    }
}

public record CommentSummaryRequest(
    int TicketId,
    int[] CommentIds,
    string Mode
);

