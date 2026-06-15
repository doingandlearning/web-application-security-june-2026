# Demo 03: API data exposure (over-sharing and object graphs)

Goal: show how APIs often leak more than intended: sensitive fields, internal ids, full object graphs, and error details.

## TrustyTickets examples to use

### 1) “Over-sharing” in responses

Call an endpoint that returns nested objects.

For example:

- Route: `GET /api/tickets/{id}`

```http
GET http://localhost:5005/api/tickets/1
```

- Code: `TrustyTickets/Controllers/TicketsController.cs` → `GetById`
- Data source: `TrustyTickets/Services/TicketService.cs` → `GetTicketByIdAsync`

Talking points:

- Returning the **entire EF entity graph** is convenient but risky.
- It can leak fields you didn’t mean to expose (e.g. internal IDs, flags, audit fields).
- It can leak sensitive fields if they exist on the model (password hashes, tokens, emails, etc.).

### 2) Password hash leakage as a “teachable shock”

In TrustyTickets, users include `PasswordHash` (deliberately insecure for teaching).

- Model: `TrustyTickets/Models/User.cs` includes `PasswordHash`
- If API responses include full `User` objects, you risk leaking it.

Key message:

- “Even if it’s ‘hashed’, you don’t ship it to clients.”

## The fix pattern (what to explain)

- Use **DTOs** (response models) so you explicitly choose fields to return.
- Avoid returning EF entities directly.

Example DTO idea:

```csharp
public record TicketDto(int Id, string Title, string Status, UserDto Owner);
public record UserDto(int Id, string DisplayName);
```

Then map entities to DTOs in the controller/service.

## Optional: “minimal response” exercise tie-in

Ask:

- “What fields does the UI actually need?”
- “What fields are internal-only?”
- “Which fields become sensitive in logs and telemetry?”

---

## Extra talking points (for deeper discussion)

- **“Hash != safe to expose”**
  - Even if a field is hashed, shipping it to the client:
    - Makes offline cracking easier if someone can capture traffic/JS,
    - Tells an attacker which accounts exist (usernames/emails),
    - Violates least privilege (“client doesn’t need this to render UI”).

- **“Internal IDs and correlations”**
  - Numeric IDs, GUIDs, tenant IDs, or correlation IDs can:
    - Aid enumeration (IDOR-style attacks),
    - Leak multi-tenant structure or environment details.
  - Often fine to use internally, but not needed in the public surface.

- **“Object graphs and cost”**
  - Returning the whole EF graph:
    - Increases payload size and latency,
    - Amplifies the blast radius if a new sensitive field is added later,
    - Makes it harder to audit what’s actually exposed.
  - DTOs are both a **security** and **performance** improvement.

- **“Versioning and ‘just adding a field’”**
  - Ask: “When someone adds a new property to `User` or `Ticket`, does anyone ask ‘is it OK to expose this to all clients?’”
  - Encourage having a **review point** (code review checklist, API design review) that explicitly calls out sensitive fields.

- **“Logs vs responses”**
  - Even if you trim responses, double-check:
    - What ends up in structured logs,
    - What goes to APM/metrics tools,
    - What appears in error traces.
  - Message: “Minimal responses + minimal logs = smaller blast radius.”

