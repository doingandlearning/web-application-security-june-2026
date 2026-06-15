using Microsoft.EntityFrameworkCore;
using TrustyTickets.Data;
using TrustyTickets.Models;

namespace TrustyTickets.Services;

/// <summary>Deliberately weak auth: plaintext comparison, verbose errors. Fix with PasswordHasher + rate limiting.</summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db) => _db = db;

    /// <summary>VULN: Plaintext password comparison; verbose "user not found" vs "wrong password" for teaching.</summary>
    public async Task<(User? user, string? error)> ValidateLoginAsync(string userName, string password, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
        if (user == null)
            return (null, "User not found"); // Verbose: reveals existence
        if (user.PasswordHash != password)
            return (null, "Invalid password"); // Verbose: confirms user exists
        return (user, null);
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken ct = default)
        => await _db.Users.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken ct = default)
        => await _db.Users.OrderBy(u => u.UserName).ToListAsync(ct);
}
