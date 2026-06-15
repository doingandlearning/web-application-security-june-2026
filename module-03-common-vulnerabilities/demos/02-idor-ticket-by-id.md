# Demo 02: IDOR — access another user’s ticket (`GET /api/tickets/{id}`)

Goal: demonstrate broken access control via “Insecure Direct Object Reference” (IDOR): the app fetches by id but doesn’t check ownership.

## Prereqs

- TrustyTickets running (see `00-setup-trustytickets.md`)
- You have `bob.cookies` (logged in as Bob)

```bash
export TT_BASE="http://localhost:5005"
```

---

## 1) Show Bob’s “legitimate” view

List tickets as Bob:

```bash
curl -s -b bob.cookies "$TT_BASE/api/tickets"
```

Talking points:

- This looks fine: the UI/endpoint appears to respect “what Bob should see”.
- Many apps stop here and assume access control is “done”.

---

## 2) Change the id and fetch someone else’s ticket

Now request a specific ticket id that belongs to someone else (seed data uses ticket `1` as Alice’s):

```bash
curl -i -s -b bob.cookies "$TT_BASE/api/tickets/1"
```

Expected result:

- HTTP `200 OK`
- Ticket `1` details are returned to Bob even though it’s not Bob’s ticket.

---

## 3) What to point out (say this)

- The app checks “are you logged in?” but never checks “do you own this resource?”.
- This is **Broken Access Control (OWASP A01)**.
- This is also a great example of why “we’ll hide the link in the UI” is not a security control.

Key takeaway:

- Fix is a **server-side ownership/role check** on every “get/update/delete by id” endpoint (Module 4).

---

## Optional: show it through the UI

Avoid hardcoding `1` here: if the DB isn’t freshly seeded, ticket IDs may not match the seed example.

**Reliable UI flow:**

1. Open a second browser profile / Incognito window.
2. In Window A, log in as **Alice** and create a new ticket (any title).
3. Open the ticket and note the URL: `ticket.html?id=<aliceTicketId>`
4. In Window B, log in as **Bob** and open any ticket so you’re on `ticket.html?id=<someId>`
5. In Window B, replace the id in the URL with `<aliceTicketId>` and reload.

Expected: Bob can view Alice’s ticket — that’s the IDOR.

