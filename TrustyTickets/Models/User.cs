namespace TrustyTickets.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    /// <summary>Deliberately insecure: plaintext or weak hash for teaching. Fix with PasswordHasher.</summary>
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Ticket> OwnedTickets { get; set; } = new List<Ticket>();
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
}
