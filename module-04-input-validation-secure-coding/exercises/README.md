# Module 4 — Exercises

## Lab 4: Fix the vulnerabilities

This lab has four core tasks plus a debrief step. Work through them in order — each fix builds on the previous one, and Task 5 (rerun and verify) only works if Tasks 1–4 are complete.

Allow 45–60 minutes. If time is tight, prioritise Tasks 1, 2, and 4 — the SQLi, XSS, and IDOR fixes. Task 3 (validation) is useful but not a prerequisite for the debrief.

---

## Scenario: you inherited a vulnerable app

TrustyTickets is intentionally vulnerable. Your job is to make it safe enough that:

- Injection payloads no longer change query behaviour.
- XSS payloads render as harmless text.
- Users cannot access other users' tickets or attachments by changing IDs.

You must not remove functionality to achieve this. Search must still work. Comments and descriptions must still display text.

**Prereq:** confirm you can reproduce at least one exploit from Module 3 before you start fixing. If the payload no longer fires before you change anything, check your lab environment.

Start the app: `cd TrustyTickets && dotnet run`

---

## Task 1: Fix SQL injection (10–15 minutes)

Fix both the ticket search and the reports search. Fixing only one does not count.

**Your task:**

Locate the vulnerable SQL concatenation in:
- `GET /api/tickets?search=` (ticket search)
- `GET /api/reports/assignees?q=` (reports search)

Replace string concatenation with an EF Core query API or parameterised SQL. Prefer the ORM approach.

**Hints:**
- Search the codebase for `FromSqlRaw` or string interpolation in SQL strings.
- An EF Core `.Where(t => t.Title.Contains(search))` call handles parameterisation automatically.
- The fix is typically one or two lines per endpoint.

**Verify:**

Rerun the Module 3 SQLi payload:

```
%' ) OR 1=1 -- 
```

It should now return only the expected results, not every row in the table.

<details>
<summary>Fix direction</summary>

```csharp
// Before
var sql = "SELECT * FROM Tickets WHERE Title LIKE '%" + search + "%'";

// After — ORM
var tickets = await db.Tickets
    .Where(t => t.OwnerId == userId && t.Title.Contains(search))
    .ToListAsync();

// After — parameterised raw SQL if ORM is not suitable
cmd.Parameters.AddWithValue("@search", "%" + search + "%");
```

</details>

---

## Task 2: Fix stored XSS (10 minutes)

Fix both the comment surface and the ticket description surface. Fixing only one does not count.

**Your task:**

Find where comments and ticket descriptions are rendered with `innerHTML`. Replace with safe rendering:

- `textContent` for plain text content
- Framework auto-escaping if you are using a templating layer
- A strict sanitiser allowlist only if you genuinely need to allow some HTML formatting — not as a shortcut

**Verify:**

Rerun the Module 3 XSS payload:

```html
<img src=x onerror=alert(1)>
```

It should now appear as visible text on the page, not execute.

**Hints:**
- Search the frontend code for `innerHTML` assignments.
- `element.textContent = value` is a one-character change in most cases.
- If the payload still fires after your fix, check whether there is a second rendering path you missed.

<details>
<summary>Fix direction</summary>

```js
// Before
commentDiv.innerHTML = comment.body;

// After
commentDiv.textContent = comment.body;
```

</details>

---

## Task 3: Add validation to one endpoint (5–8 minutes)

**Your task:**

Pick one input surface — ticket title, search string, or assign request — and add basic validation:

- Type check — is this the expected type?
- Length limit — reject inputs that exceed a reasonable maximum.
- Format constraint — where relevant, e.g. an ID should be a positive integer.

Return a 400 with a clear, generic error message on invalid input. Do not return a stack trace.

**Discussion point to bring to the debrief:**

Validation improves stability and UX. It is not a substitute for parameterisation or output encoding — a string that passes validation can still be used in a SQL injection if the query is not parameterised.

---

## Task 4: Fix IDOR (10 minutes)

Fix both the ticket-by-id endpoint and the attachment download endpoint. Fixing only one does not count.

**Your task:**

Add an ownership or role check to:
- `GET /api/tickets/{id}`
- `GET /api/attachments/{id}/download`

**Policy decision:** choose whether unauthorised access returns 403 or 404, and apply it consistently across both endpoints. Write a one-line rationale in your notes: "We chose X because…"

**Verify:**

Log in as `bob`. Request a ticket or attachment owned by `alice` by changing the ID. You should receive a 403 or 404, not the resource.

**Hints:**
- The fix is an ownership check after the resource is fetched: compare `ticket.OwnerId` to the current user's ID.
- If the resource does not exist, return 404. If it exists but the user is not authorised, your policy choice applies.
- Admin users should be exempt from the ownership check — check for role before enforcing ownership.

<details>
<summary>Fix direction</summary>

```csharp
var ticket = await db.Tickets.FindAsync(id);

if (ticket == null)
    return NotFound();

if (ticket.OwnerId != currentUserId && !currentUser.IsAdmin)
    return Forbid();   // 403

return Ok(ticket);
```

</details>

---

## Task 5: Rerun and verify (5 minutes)

Run all three Module 3 payloads against your fixed app. All three should now fail to exploit.

| Payload | Expected result after fix |
|---|---|
| `%' ) OR 1=1 --` in the search field | Normal results only — no extra rows |
| Change ticket ID to another user's ID | 403 or 404 — not the resource |
| `<img src=x onerror=alert(1)>` in a comment | Renders as visible text — no alert |

If any payload still succeeds, you have a second instance of the vulnerability that the fix did not cover. Check both surfaces for XSS and both endpoints for IDOR.

---

## Debrief (chat share)

Type in chat: **your one-line rationale for 403 vs 404 on the IDOR fix**.

The facilitator will read a few and ask: did the group reach a consistent policy? If different people chose differently — what drove the decision?

---

## Stretch goals

Pick one if you finish early:

1. **Consistency pass** — find one additional endpoint that takes an `id` and confirm it enforces the same ownership check. If it does not, fix it.
2. **Better error handling** — replace any 500 responses caused by bad input with clean 400s.
3. **Regression note** — write one sentence: "To stop this vulnerability returning in a future PR, we would…" (a SAST rule, a code review checklist item, or a CI gate).

---

## Expected output

- Fixed code covering both surfaces for each vulnerability type.
- The Module 3 verification table completed — all three payloads confirmed to fail.
- A one-line IDOR policy decision (403 vs 404) with a rationale.

---

## Key concepts

- **Parameterisation** prevents SQL injection — sanitisation does not.
- **Safe rendering** prevents XSS — sanitisation alone is not reliable.
- **Validation** improves robustness but is not a security control for injection.
- **Server-side ownership checks** prevent IDOR — client-side filtering does not.

---

## Next steps

Module 5 covers session management and data protection — hardening how identity is maintained between requests and how data is protected in transit and at rest. The same TrustyTickets app is the reference point throughout.