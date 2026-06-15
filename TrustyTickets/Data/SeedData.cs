using Microsoft.EntityFrameworkCore;
using TrustyTickets.Models;

namespace TrustyTickets.Data;

public static class SeedData
{
    /// <summary>Plaintext passwords for teaching only. Fix with proper hashing.</summary>
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        if (await db.Users.AnyAsync()) return;

        var alice = new User
        {
            UserName = "alice",
            PasswordHash = "alice123",
            DisplayName = "Alice",
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow
        };
        var bob = new User
        {
            UserName = "bob",
            PasswordHash = "bob456",
            DisplayName = "Bob",
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow
        };
        var admin = new User
        {
            UserName = "admin",
            PasswordHash = "admin",
            DisplayName = "Admin",
            IsAdmin = true,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.AddRange(alice, bob, admin);
        await db.SaveChangesAsync();

        var t1 = new Ticket
        {
            Title = "Broken login",
            Description = "Users report login fails after password change.",
            Status = "Open",
            OwnerId = alice.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var t2 = new Ticket
        {
            Title = "Slow dashboard",
            Description = "Dashboard loads slowly with many tickets.",
            Status = "In Progress",
            OwnerId = bob.Id,
            AssignedToId = alice.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Tickets.AddRange(t1, t2);
        await db.SaveChangesAsync();

        db.Comments.AddRange(
            new Comment { TicketId = t1.Id, AuthorId = alice.Id, Body = "Happens on Chrome only.", CreatedAt = DateTime.UtcNow },
            new Comment { TicketId = t1.Id, AuthorId = bob.Id, Body = "I'll check the logs.", CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        // One attachment for IDOR lab: download by ID without ownership check
        db.Attachments.Add(new Attachment
        {
            TicketId = t1.Id,
            FileName = "screenshot.png",
            ContentType = "image/png",
            Content = System.Text.Encoding.UTF8.GetBytes("(placeholder image content for ticket 1)"),
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}
