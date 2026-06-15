# Demo 02: Fix IDOR (ownership checks) + re-test

Goal: show how “logged in” is not the same as “authorised”, and how ownership checks stop IDOR.

## What you’ll change

### Files

- Ticket endpoint: `TrustyTickets/Controllers/TicketsController.cs` → `GetById`
- Attachment endpoint: `TrustyTickets/Controllers/AttachmentsController.cs` → `Download`
- Data access: `TrustyTickets/Services/TicketService.cs` (currently returns any ticket/attachment by id)

### Routes

- `GET /api/tickets/{id}` (teaching IDOR)
- `GET /api/attachments/{id}/download` (lab IDOR)

## Fix approach (recommended for the demo)

Add server-side checks that enforce:

- User must be logged in
- User must either:
  - own the ticket, or
  - be assigned to the ticket, or
  - be admin

### Ticket-by-id: code sketch

In `TicketsController.GetById`:

1. Determine current user id
2. Load the ticket
3. Enforce ownership/assignment/admin before returning

Conceptual pattern:

```csharp
if (ticket.OwnerId != userId && ticket.AssignedToId != userId && !isAdmin)
    return Forbid();
```

### Attachment download: code sketch

For attachments you need to check the attachment’s associated ticket before allowing download.

Pattern:

- Load attachment with its TicketId
- Load that ticket (or join) and enforce the same ownership/assignment/admin logic

## Prove it’s fixed (rerun the same trick)

### Before (baseline)

- Log in as Bob
- View a ticket
- Change `ticket.html?id=...` to an Alice ticket id

Expected: Bob sees Alice’s data.

### After

Repeat:

Expected: 403 (or 404 based on your chosen policy), and the UI can’t display the other user’s ticket.

Repeat for attachments:

- As Bob, try `/api/attachments/<id>/download` for an attachment not belonging to Bob.

Expected: blocked.

## Talking points

- “ID from the client” is always untrusted.
- Fix lives on the server, not the UI.
- This is the most common real-world web vuln category: **Broken Access Control**.

