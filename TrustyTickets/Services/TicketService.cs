using Microsoft.EntityFrameworkCore;
using TrustyTickets.Data;
using TrustyTickets.Models;

namespace TrustyTickets.Services;

/// <summary>Deliberately vulnerable service: SQLi, IDOR. Fix during course.</summary>
public class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db) => _db = db;

    /// <summary>VULN (teaching): SQL Injection via search. Use raw SQL concatenation.</summary>
    public async Task<IEnumerable<Ticket>> GetTicketsForUserAsync(int userId, bool isAdmin, string? search, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            var query = _db.Tickets.Include(t => t.Owner).Include(t => t.AssignedTo).AsQueryable();
            if (!isAdmin) query = query.Where(t => t.OwnerId == userId || t.AssignedToId == userId);
            return await query.OrderByDescending(t => t.UpdatedAt).ToListAsync(ct);
        }

        // Deliberately vulnerable: string concatenation into raw SQL
        // Intentionally NO escaping/parameterization here — fixed in later modules.
        var searchRaw = search;
        var sql = $@"
            SELECT t.""Id"", t.""Title"", t.""Description"", t.""Status"", t.""OwnerId"", t.""AssignedToId"", t.""CreatedAt"", t.""UpdatedAt""
            FROM ""Tickets"" t
            INNER JOIN ""Users"" o ON t.""OwnerId"" = o.""Id""
            WHERE (t.""OwnerId"" = {userId} OR t.""AssignedToId"" = {userId} OR {(isAdmin ? "1=1" : "0=1")})
            AND (t.""Title"" LIKE '%{searchRaw}%' OR t.""Description"" LIKE '%{searchRaw}%')";
        return await _db.Tickets
            .FromSqlRaw(sql)
            .Include(t => t.Owner)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(ct);
    }

    /// <summary>VULN (teaching): IDOR — no ownership check; return any ticket.</summary>
    public async Task<Ticket?> GetTicketByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Tickets
            .Include(t => t.Owner)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments).ThenInclude(c => c.Author)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    /// <summary>VULN (lab): SQL Injection in reports. Same bad pattern as search.</summary>
    public async Task<IEnumerable<string>> GetAssigneeNamesAsync(string? q, CancellationToken ct = default)
    {
        var filter = q ?? "";
        // Deliberately vulnerable: raw SQL with concatenation
        var sql = $@"SELECT ""UserName"" FROM ""Users"" WHERE ""UserName"" LIKE '%{filter}%'";
        var query = _db.Database.SqlQueryRaw<string>(sql);
        return await query.ToListAsync(ct);
    }

    public async Task<Ticket?> CreateTicketAsync(int ownerId, string title, string description, CancellationToken ct = default)
    {
        var ticket = new Ticket
        {
            Title = title,
            Description = description,
            Status = "Open",
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(ct);
        return await GetTicketByIdAsync(ticket.Id, ct);
    }

    /// <summary>CSRF lab: state-changing, no anti-forgery.</summary>
    public async Task<bool> AssignTicketAsync(int ticketId, int assigneeId, CancellationToken ct = default)
    {
        var ticket = await _db.Tickets.FindAsync(new object[] { ticketId }, ct);
        if (ticket == null) return false;
        ticket.AssignedToId = assigneeId;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Comment?> AddCommentAsync(int ticketId, int authorId, string body, CancellationToken ct = default)
    {
        var comment = new Comment
        {
            TicketId = ticketId,
            AuthorId = authorId,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(ct);
        return comment;
    }

    /// <summary>VULN (lab): IDOR — return any attachment by ID, no ownership check.</summary>
    public async Task<Attachment?> GetAttachmentByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Attachments.FindAsync(new object[] { id }, ct);
    }
}
