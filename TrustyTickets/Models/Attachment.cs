namespace TrustyTickets.Models;

/// <summary>Download by ID without ownership check — IDOR lab surface.</summary>
public class Attachment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public DateTime CreatedAt { get; set; }
}
