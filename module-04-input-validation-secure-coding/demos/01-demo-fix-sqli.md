# Demo 01: Fix SQL injection (parameterise / ORM) + re-test payload

Goal: show “this one change stops the exploit” and explain why parameterisation works.

## What you’ll change

### Files

- Vulnerable service: `TrustyTickets/Services/TicketService.cs`
- Routes that depend on it:
  - `GET /api/tickets?search=...` (teaching)
  - `GET /api/reports/assignees?q=...` (lab)

### Why it’s vulnerable

`TicketService` concatenates user input directly into raw SQL.

## Fix approach (recommended for the demo)

Use EF Core query APIs for search instead of raw SQL.

### Code changes (copy/paste)

In `TrustyTickets/Services/TicketService.cs`, replace the raw SQL block in `GetTicketsForUserAsync` with an EF query like:

```csharp
var query = _db.Tickets
    .Include(t => t.Owner)
    .Include(t => t.AssignedTo)
    .AsQueryable();

if (!isAdmin)
{
    query = query.Where(t => t.OwnerId == userId || t.AssignedToId == userId);
}

query = query.Where(t => t.Title.Contains(search) || t.Description.Contains(search));

return await query
    .OrderByDescending(t => t.UpdatedAt)
    .ToListAsync(ct);
```

For the reports SQLi in `GetAssigneeNamesAsync`, use a parameterised LIKE:

```csharp
var filter = $"%{q ?? ""}%";
return await _db.Users
    .Where(u => EF.Functions.Like(u.UserName, filter))
    .Select(u => u.UserName)
    .ToListAsync(ct);
```

## Prove it’s fixed (rerun the same payload)

### Before (baseline)

In Module 3 you could use:

```text
%' ) OR 1=1 -- 
```

to pull back extra results.

### After

Rerun the same payload in:

- `tickets.html` search box, and/or
- `reports.html` filter box

Expected:

- No SQL errors.
- No extra rows.
- Payload is treated as a literal search string.

## Talking points

- The “fix” is not regex cleansing; it’s changing how the query is constructed.
- Parameterised/ORM queries keep data as data.
- The exploit worked because input controlled SQL structure; the fix prevents that.

---

## “We’d never do that” — realistic C# gotchas

If (when) someone says “we’d never build SQL like that”, you can walk through some **less cartoonish** patterns that still open doors:

1. **Admin/ops tools and reports**

   ```csharp
   // Internal admin tool — “only admins can use it”
   var sql = $"SELECT * FROM AuditLog WHERE UserName = '{userName}' ORDER BY CreatedAt DESC";
   ```

   - Often written quickly for an internal team.
   - AuthZ may be tight, but any bug or future reuse of this method becomes an injection sink.

2. **Search/filter helpers**

   ```csharp
   var where = "";
   if (!string.IsNullOrEmpty(filter.Status))
       where += $" AND Status = '{filter.Status}'";
   if (!string.IsNullOrEmpty(filter.Owner))
       where += $" AND OwnerName LIKE '%{filter.Owner}%'";

   var sql = "SELECT * FROM Tickets WHERE 1=1" + where;
   ```

   - Starts as “a small helper”.
   - Grows over time; every new filter is another concatenation.

3. **“It’s just an IN clause”**

   ```csharp
   var idsCsv = string.Join(",", ids); // ids may come from query/body
   var sql = $"SELECT * FROM Orders WHERE Id IN ({idsCsv})";
   ```

   - Looks harmless if you assume `ids` are all integers.
   - Any path where `ids` isn’t fully server‑derived opens injection.

4. **Dynamic ORDER BY / column selection**

   ```csharp
   // sortField is from query string: "CreatedAt", "Title", etc.
   var sql = $"SELECT * FROM Tickets ORDER BY {sortField} DESC";
   ```

   - No quotes, but still injection: attacker can inject `CreatedAt; DROP TABLE Users; --`.
   - Fix is an allowlist of valid column names, not free‑form strings.

5. **Using `FromSqlRaw` with interpolated strings**

   ```csharp
   // EF Core call, but still raw SQL
   var users = await db.Users
       .FromSqlRaw($"SELECT * FROM Users WHERE UserName LIKE '%{term}%'")
       .ToListAsync();
   ```

   - Looks like “just EF”.
   - Vulnerable for the same reason: interpolated string + user input.

You don’t need to deep‑dive every example; the point is to show that **“we’d never do that” often means “we don’t notice when we do”**—especially in admin tools, reports, and helper methods.

