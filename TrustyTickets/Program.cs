using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TrustyTickets.Data;
using TrustyTickets.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        // TrustyTickets uses EF navigation properties for teaching.
        // Ignore cycles so demo endpoints serialize instead of throwing 500s.
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db"));
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// No antiforgery for CSRF teaching; add when fixing
builder.Services.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();

// Prevent noisy 404s: browsers often request /favicon.ico automatically.
// Serve an SVG favicon and redirect /favicon.ico to it.
app.MapGet("/favicon.ico", () => Results.Redirect("/favicon.svg", permanent: true));

// SPA fallback so /login and /tickets work
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.SeedAsync(db);
}

app.Run();
