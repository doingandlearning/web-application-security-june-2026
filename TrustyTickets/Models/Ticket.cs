namespace TrustyTickets.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    /// <summary>Stored and rendered with innerHTML — XSS lab surface.</summary>
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
