using TrustyTickets.Models;

namespace TrustyTickets.Services;

public interface ITicketService
{
    Task<IEnumerable<Ticket>> GetTicketsForUserAsync(int userId, bool isAdmin, string? search, CancellationToken ct = default);
    Task<Ticket?> GetTicketByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<string>> GetAssigneeNamesAsync(string? q, CancellationToken ct = default);
    Task<Ticket?> CreateTicketAsync(int ownerId, string title, string description, CancellationToken ct = default);
    Task<bool> AssignTicketAsync(int ticketId, int assigneeId, CancellationToken ct = default);
    Task<Comment?> AddCommentAsync(int ticketId, int authorId, string body, CancellationToken ct = default);
    Task<Attachment?> GetAttachmentByIdAsync(int id, CancellationToken ct = default);
}
