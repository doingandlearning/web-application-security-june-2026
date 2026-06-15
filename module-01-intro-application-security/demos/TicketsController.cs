using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Module01.Demos
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // assume auth is required for tickets
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public TicketsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // DEMO 1: Vulnerable version for "what could go wrong?" discussion
        //
        // GET /api/tickets?id=123
        [HttpGet("vulnerable")]
        public async Task<IActionResult> GetTicketVulnerable([FromQuery] int id)
        {
            var ticket = await _db.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        // DEMO 2: Fixed version showing ownership/role checks
        //
        // GET /api/tickets/secure?id=123
        [HttpGet("secure")]
        public async Task<IActionResult> GetTicketSecure([FromQuery] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var ticket = await _db.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // Enforce ownership (or admin role)
            if (ticket.OwnerId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return Ok(ticket);
        }
    }

    // Minimal placeholder to make the demo compile in isolation.
    public class ApplicationDbContext
    {
        public Task<Ticket> FindAsync(int id) => Task.FromResult<Ticket>(null);
        public DbSet<Ticket> Tickets { get; set; }
    }

    public class DbSet<T>
    {
        public Task<T> FindAsync(int id) => Task.FromResult<T>(default);
    }

    public class Ticket
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}