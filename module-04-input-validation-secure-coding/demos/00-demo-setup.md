# Demo 00: Setup + baseline (prove the vulns exist)

Goal: before you start fixing, show that the same payloads from Module 3 still work. This makes the “after” proof meaningful.

## App

Use TrustyTickets.

- App README: `TrustyTickets/README.md`
- Run: `cd TrustyTickets && dotnet run`

Default users:

- `alice` / `alice123`
- `bob` / `bob456`
- `admin` / `admin`

## Baseline checks (show quickly)

### SQLi (teaching): ticket search

- UI route: `TrustyTickets/wwwroot/tickets.html` (search box)
- API route: `GET /api/tickets?search=...`
- Vulnerable code: `TrustyTickets/Services/TicketService.cs` → `GetTicketsForUserAsync`

Try payload (as Bob):

```text
%' ) OR 1=1 -- 
```

Expected: results include tickets Bob should not see.

### IDOR (teaching): ticket by id

- UI route: `TrustyTickets/wwwroot/ticket.html?id=...`
- API route: `GET /api/tickets/{id}`
- Vulnerable code:
  - `TrustyTickets/Controllers/TicketsController.cs` → `GetById`
  - `TrustyTickets/Services/TicketService.cs` → `GetTicketByIdAsync`

Expected: Bob can view Alice’s ticket by changing the `id`.

### Stored XSS (teaching): comments

- UI route: `TrustyTickets/wwwroot/ticket.html`
- API route: `POST /api/tickets/{id}/comments`
- Vulnerable code: `TrustyTickets/wwwroot/ticket.html` → `renderComment` uses `innerHTML`

Paste into comment box:

```html
<img src=x onerror=alert(1)>
```

Expected: alert (or visible DOM change) when the ticket renders comments.

## Talking points

- “We’re not fixing yet; we’re confirming the baseline.”
- “After each fix, we’ll rerun the same payload to prove the vulnerability is gone.”

