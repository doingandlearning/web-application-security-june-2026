# TrustyTickets — Deliberately Vulnerable App

ASP.NET Core (C#) + vanilla JS app for the Application Security course. It is **intentionally vulnerable**; you fix it as the course progresses.

## Tech stack

- **Backend:** ASP.NET Core 9, EF Core, SQLite
- **Frontend:** Vanilla JS, static HTML/CSS
- **Auth:** Cookie-based session (no anti-forgery initially)

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (or .NET 8)

## Run

```bash
cd TrustyTickets
dotnet run
```

Open http://localhost:5000 (or the URL shown). Default users: `alice` / `alice123`, `bob` / `bob456`, `admin` / `admin`.

## Vulnerability map (teaching vs lab)

| Vulnerability | Teaching | Lab |
|---------------|----------|-----|
| **SQL Injection** | `GET /api/tickets?search=` | `GET /api/reports/assignees?q=` |
| **IDOR** | `GET /api/tickets/{id}` | `GET /api/attachments/{id}/download` |
| **Stored XSS** | Comments (innerHTML) | Ticket description (innerHTML) |
| **CSRF** | `POST /api/tickets` | `POST /api/tickets/{id}/assign` |
| **Auth** | Login: plaintext, verbose errors | Same; fix in lab |
| **Headers/CSP** | None | Add in lab |

## Project layout

- **Controllers/** — Auth, Tickets, Reports, Attachments, Users
- **Services/** — TicketService (vulnerable SQLi/IDOR), AuthService (weak auth)
- **Models/** — User, Ticket, Comment, Attachment
- **Data/** — AppDbContext, SeedData
- **wwwroot/** — login.html, tickets.html, ticket.html, reports.html, js/api.js

## Database

SQLite file `app.db` is created on first run. Seed data: 3 users, 2 tickets, comments, 1 attachment. Delete `app.db` to reseed.
