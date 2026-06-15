namespace TrustyTickets.Models;

/// <summary>Rendered with innerHTML in frontend — XSS teaching surface.</summary>
public class Comment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
