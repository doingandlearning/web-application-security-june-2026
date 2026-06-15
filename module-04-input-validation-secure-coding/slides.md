---
title: "**Introduction to Application Security**"
sub_title: Module 4 — Input Validation and Secure Coding
author: Kevin Cunningham
---

## The code from Module 3

```csharp
var sql = "SELECT * FROM Tickets WHERE Title LIKE '%" + search + "%'";
```

```js
commentDiv.innerHTML = userComment;
```

```csharp
var ticket = await db.Tickets.FindAsync(id);
return Ok(ticket);
```

<!-- pause -->

**Type in chat — which of these is hardest to fix correctly?**

By the end of this module you will have fixed all three. We'll rerun the Module 3 payloads to confirm.

<!-- speaker_note: Most will say the ownership check (third one) because the fix requires understanding the auth model. Some will say the SQL because they've heard sanitisation is hard. Both are reasonable — the point is that all three have a specific, learnable pattern. -->

<!-- end_slide -->

<!-- jump_to_middle -->

SQL injection
===

<!-- end_slide -->

## The rule and the pattern

**Rule:** user input is data. It is never code. Treat it accordingly at every database boundary.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Unsafe — string concatenation**

```csharp
var sql =
  "SELECT * FROM Tickets " +
  "WHERE Title LIKE '%" +
  search + "%'";
```

The value of `search` becomes part of the query structure. An attacker controls the structure.

<!-- column: 1 -->

**Safe — ORM query API**

```csharp
var tickets = await db.Tickets
  .Where(t =>
    t.OwnerId == userId &&
    t.Title.Contains(search))
  .ToListAsync();
```

`search` is passed as a value. The ORM handles parameterisation.

<!-- reset_layout -->

<!-- end_slide -->

## When you need raw SQL

Sometimes an ORM query won't do. Raw SQL is fine — with parameters.

```csharp
using var cmd = new SqlCommand(
    "SELECT * FROM Tickets " +
    "WHERE OwnerId = @ownerId AND Title LIKE @search",
    conn);

cmd.Parameters.AddWithValue("@ownerId", ownerId);
cmd.Parameters.AddWithValue("@search", "%" + search + "%");
```

<!-- pause -->

The parameter value is never interpreted as SQL syntax. The query structure is fixed at compile time.

<!-- pause -->

**Why sanitisation is not the answer**

Regex and blocklists are brittle — there are always encodings and edge cases that bypass them. Escaping is easy to get wrong and depends on database context. Parameterisation makes the question irrelevant — the input cannot become code regardless of what it contains.

<!-- end_slide -->

<!-- jump_to_middle -->

Cross-site scripting
===

<!-- end_slide -->

## The rule and the pattern

**Rule:** untrusted input is text. Render it as text. Never hand it to the browser as HTML or JavaScript.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Unsafe — innerHTML**

```js
commentDiv.innerHTML = userComment;
```

The browser parses `userComment` as HTML. Any `<script>` tag or event handler in it executes.

<!-- column: 1 -->

**Safe — textContent**

```js
commentDiv.textContent = userComment;
```

The browser renders `userComment` as a literal string. Angle brackets appear as text, not as tags.

<!-- reset_layout -->

<!-- pause -->

**If you genuinely need to render HTML** — a rich text editor, formatted content — use a strict sanitisation library (DOMPurify) rather than rolling your own allowlist. And still encode anything going into attributes or script contexts separately.

<!-- end_slide -->

## Encoding by context

The same untrusted string needs different treatment depending on where it goes.

<!-- pause -->

| Output context | Risk | Safe approach |
|---|---|---|
| HTML body | Tags interpreted as markup | `textContent` or auto-escaping template |
| HTML attribute | Attribute injection, event handlers | Attribute encoding — `&quot;` not raw `"` |
| URL parameter | Open redirect, injection | `encodeURIComponent` |
| JavaScript string | Script injection | JSON encoding or avoid entirely |

<!-- pause -->

**Escape on output, not on input.** Cleaning input strips data you might need elsewhere. Encoding output for the specific context is precise and safe.

<!-- end_slide -->

<!-- jump_to_middle -->

Validation, sanitisation, and what each one does
===

<!-- end_slide -->

## Two different jobs — don't confuse them

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Validation**
Is this input allowed?

Type, length, format, business rules.

Rejects bad input early — improves stability and UX.

Does not prevent injection. A string that passes validation can still be used in a SQL attack.

<!-- column: 1 -->

**Encoding and parameterisation**
Make this data safe for where it is going.

Context-specific — HTML encoding, URL encoding, SQL parameters.

Prevents injection regardless of whether the input passed validation.

<!-- reset_layout -->

<!-- pause -->

**Type in chat — if you validate that a search field contains only letters and spaces, does that prevent SQL injection?**

<!-- speaker_note: The answer is "it reduces the risk but does not eliminate it" — there are injection techniques that use only alphabetic characters in some contexts. More importantly, relying on validation as the injection control means one missed validation = vulnerability. Parameterisation means the input cannot become code regardless. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Authentication and authorisation
===

<!-- end_slide -->

## Two different questions — don't conflate them

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Authentication**
Who is this?

Login, session token, JWT verification.

Establishes identity — "this request comes from user 42".

<!-- column: 1 -->

**Authorisation**
Are they allowed?

Ownership checks, role checks, permission policies.

Enforces access — "user 42 can only read their own tickets".

<!-- reset_layout -->

<!-- pause -->

Most real-world breaches are authorisation failures, not authentication failures. The user is logged in. They just access data they should not be able to reach.

<!-- pause -->

**Type in chat — in your current codebase, is the ownership check in the controller, the service layer, or somewhere else?**

<!-- speaker_note: This question often produces "I'm not sure" — which is useful. If the team does not know where the ownership check lives, they cannot be confident it is consistently applied. The next slide addresses exactly this. -->

<!-- end_slide -->

## Fixing IDOR — the ownership check pattern

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Unsafe — no ownership check**

```csharp
var ticket = await db.Tickets
    .FindAsync(id);

return Ok(ticket);
```

Any authenticated user can read any ticket by supplying its ID.

<!-- column: 1 -->

**Safe — ownership enforced**

```csharp
var ticket = await db.Tickets
    .FindAsync(id);

if (ticket == null)
    return NotFound();

if (ticket.OwnerId != currentUserId
    && !currentUser.IsAdmin)
    return Forbid();

return Ok(ticket);
```

<!-- reset_layout -->

<!-- end_slide -->

## Where should the ownership check live?

<!-- column_layout: [2, 3] -->

<!-- column: 0 -->

**Controller level**
Easy to add. Easy to forget on a new endpoint. Easy to skip under time pressure.

**Service or policy layer**
Hard to forget — access goes through one path. New endpoints inherit the check automatically.

**Both with tests**
The most reliable pattern — architectural enforcement plus regression tests.

<!-- column: 1 -->

**The test you need**

```csharp
[Fact]
public async Task GetTicket_OtherUser_ReturnsForbid()
{
    // Arrange: ticket owned by userA
    // Act: request as userB
    // Assert: 403 Forbidden
}
```

If this test exists, a future change that removes the ownership check will fail the build before it ships.

<!-- reset_layout -->

<!-- end_slide -->

## Other IDOR causes

The ownership check is necessary but not always sufficient.

<!-- pause -->

<!-- incremental_lists: true -->

- **Sequential integer IDs** — `/tickets/101`, `/tickets/102` — make enumeration trivial; an attacker does not need to guess, just increment. Use UUIDs for publicly exposed identifiers.
- **List endpoints without user filtering** — an API that returns all records and relies on the frontend to filter is not filtering. The filter belongs in the query.
- **Inconsistent checks** — a check on GET but not on PUT or DELETE. Each HTTP method on each resource needs its own enforcement.

<!-- incremental_lists: false -->

<!-- pause -->

Write the test "user B cannot read, update, or delete user A's resource" for every resource type. If it is hard to write, the authorisation model is unclear.

<!-- end_slide -->

<!-- jump_to_middle -->

Lab — fix TrustyTickets
===

<!-- end_slide -->

## Lab setup

You are fixing the same application you exploited in Module 3. Work in pairs. Use the same lab environment.

Task sheet is in `exercises/module-4/`. Each fix has a hint and an optional solution reference.

<!-- end_slide -->

## Lab targets

Complete in order — each fix builds on the previous one.

<!-- pause -->

**Fix 1 — SQL injection in ticket search**
Replace string concatenation with a parameterised query or ORM equivalent. Rerun the Module 3 SQLi payload — it should now return no results or an error, not the exploited data.

<!-- pause -->

**Fix 2 — Stored XSS in comments**
Replace `innerHTML` with `textContent` (or the framework equivalent). Rerun the Module 3 XSS payload — the script tag should render as visible text, not execute.

<!-- pause -->

**Fix 3 — IDOR on ticket-by-id**
Add an ownership check before returning the ticket. Rerun the Module 3 IDOR test — requesting another user's ticket ID should return 403, not the ticket.

<!-- pause -->

**Fix 4 (extension) — Add a validation rule**
Add length and type validation on at least one input. Confirm that invalid input returns a 400 with a generic error message, not a stack trace.

<!-- speaker_note: Circulate and prompt. Common sticking points — Fix 1: teams try to sanitise the input first rather than going straight to parameterisation. Fix 2: teams use a sanitisation library instead of textContent and the XSS still fires via an edge case. Fix 3: teams add the check but forget the DELETE and PUT routes. The extension task often surfaces that validation and injection prevention are the same thing in the team's mental model — a good moment to revisit the earlier slide. -->

<!-- end_slide -->

## Back to the three code snippets

```csharp
var sql = "SELECT * FROM Tickets WHERE Title LIKE '%" + search + "%'";
```
→ parameterised query or ORM `.Contains()`. The input cannot become SQL syntax.

<!-- pause -->

```js
commentDiv.innerHTML = userComment;
```
→ `textContent`. The input cannot become HTML or JavaScript.

<!-- pause -->

```csharp
var ticket = await db.Tickets.FindAsync(id);
return Ok(ticket);
```
→ ownership check before `return Ok`. The authenticated user can only reach their own data.

<!-- pause -->

You said: **which is hardest to fix correctly?**

The ownership check is hardest — not because the code is complex, but because it requires a consistent architectural decision about where enforcement lives. One missed endpoint undoes the fix.

<!-- speaker_note: Return to the chat poll. The point is that the difficulty is not in the line of code — it's in the consistency. Parameterisation is a local change. Authorisation enforcement is a system-wide decision. -->

<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **SQL injection** — parameterised queries and ORM APIs prevent injection; sanitisation does not
2. **XSS** — `textContent` and context-aware encoding prevent injection; `innerHTML` with untrusted input does not
3. **Validation vs encoding** — validation rejects bad input; encoding makes input safe for its destination; both are needed, neither replaces the other
4. **IDOR** — ownership checks at every endpoint, in a shared layer, with tests that would catch a regression

<!-- end_slide -->

## Bridge to Module 5

**We've established:**

<!-- incremental_lists: true -->

- The specific code patterns that prevent the vulnerabilities from Module 3
- Why parameterisation and safe rendering work where sanitisation does not
- Where ownership enforcement belongs and how to test it

<!-- incremental_lists: false -->

**Module 5 — Session management and data protection:** how identity is maintained between requests, how data is protected in transit and at rest, and how to harden the APIs those fixes now sit behind.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===

*Introduction to Application Security — Module 4*