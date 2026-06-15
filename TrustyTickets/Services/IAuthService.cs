using TrustyTickets.Models;

namespace TrustyTickets.Services;

public interface IAuthService
{
    /// <summary>Returns user if password matches. Deliberately verbose errors for teaching.</summary>
    Task<(User? user, string? error)> ValidateLoginAsync(string userName, string password, CancellationToken ct = default);
    Task<User?> GetUserByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken ct = default);
}
