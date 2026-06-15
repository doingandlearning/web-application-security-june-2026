# Module 5 — Exercises

## Lab 5: Session and data protection

This lab has two required tasks and two extension tasks. Everyone does Tasks 1 and 2. Tasks 3 and 4 are for delegates who finish early or want to apply the concepts to their own system.

Allow 30–40 minutes for Tasks 1 and 2 plus the debrief.

---

## Scenario: TrustyTickets in production

TrustyTickets is approaching a production deployment. Session handling and API responses have not been reviewed for security. Your job is to identify what needs hardening before it ships.

---

## Task 1: Harden the session cookie (10–15 minutes)

### Step 1: Inspect (5 minutes)

Open TrustyTickets and log in. Open browser dev tools → Application → Cookies.

Find the session cookie and note:
- Its name
- Which flags are set: `HttpOnly`, `Secure`, `SameSite`, `Max-Age` or `Expires`
- Which flags are absent

Then open the code:
- File: `TrustyTickets/Controllers/AuthController.cs`
- Endpoint: `POST /api/auth/login`

Confirm what the code sets versus what the browser shows.

### Step 2: Decide

For each missing or misconfigured flag, decide what it should be in production and write a one-line rationale.

Use this as your decision record:

| Flag | Current value | Production value | Why |
|---|---|---|---|
| `HttpOnly` | | | |
| `Secure` | | | |
| `SameSite` | | | |
| `Max-Age` / expiry | | | |

**Decision to make:** which `SameSite` value is appropriate for TrustyTickets — `Lax`, `Strict`, or `None`? Write one sentence justifying your choice.

**Hints:**
- `HttpOnly` prevents JavaScript from reading the cookie — protects against XSS-based session theft.
- `Secure` ensures the cookie is only sent over HTTPS — required in production, can be off in local dev.
- `SameSite=Lax` is a safe default for most app sessions; `Strict` can break OAuth redirect flows.
- A missing expiry means the cookie lives until the browser is closed — usually not what you want.

<details>
<summary>Likely findings in TrustyTickets</summary>

```
HttpOnly: absent — JavaScript can read the session cookie, enabling theft via XSS
Secure: absent — cookie sent over HTTP in dev; must be set for production
SameSite: absent — no CSRF mitigation in place
Expiry: absent — session cookie expires only when browser closes
```

</details>

---

## Task 2: API response review (10–15 minutes)

### Step 1: Inspect

Make a request to:

```
GET /api/tickets/{id}
```

Note every field in the response. Then open:
- `TrustyTickets/Controllers/TicketsController.cs`
- `TrustyTickets/Services/TicketService.cs`

### Step 2: Classify

For each field in the response, mark it as one of:

- **Required** — the UI or client genuinely needs this to function
- **Sensitive** — internal data, PII, or data that should be restricted to certain roles
- **Unnecessary** — not used by the client; no reason to expose it

### Step 3: Decide

- Name at least one field you would remove or restrict, and write one sentence explaining why.
- Identify one potential abuse scenario for this endpoint — enumeration, brute force, or over-broad data access — and name one control that would reduce the risk.

**Hints:**
- Fields like internal audit timestamps, system IDs, and raw status codes are often returned by default and rarely needed by clients.
- An endpoint that returns the full user object when only the username is needed is exposing data unnecessarily.
- Rate limiting and opaque IDs (UUIDs rather than sequential integers) address enumeration risk.

---

## Debrief (chat share)

After Tasks 1 and 2, type in chat:

**"One thing I would fix in TrustyTickets before it goes to production — and why."**

One sentence. Be specific — name the field, flag, or endpoint.

The facilitator will read a few responses. Common themes will be grouped: session configuration gaps, over-broad API responses, missing rate limiting.

---

## Extension Task 3: JWT hygiene checklist

Use this if you work on a system that uses JWTs or other tokens.

For your system, answer each question and write a one-line response:

1. How long do access tokens live?
2. What claims are in the payload — any PII or secrets?
3. Where are tokens stored on the client — cookie, `localStorage`, or platform secure storage?
4. Is the signature verified server-side on every request?
5. How are tokens revoked — short expiry, refresh rotation, or a blacklist?

Produce a 4–5 point checklist you could share with your team as a baseline standard.

**The two questions most likely to reveal a gap:** token lifetime (commonly too long) and payload contents (commonly includes email or other PII that should not be there).

---

## Extension Task 4: Log and error audit

Use this if you have access to your own codebase during the session.

Pick one logging or error-handling path. Identify:
- What is logged on success and on error
- Whether any log entry might contain passwords, tokens, session IDs, or PII

Then write:
- One "never log X" rule: "We never log [specific thing]."
- One concrete change: what you would mask, remove, or replace

**Common findings:** full email addresses in debug logs, raw query strings in error responses, session tokens in access logs, stack traces returned to the client.

---

## Expected output

- A completed cookie flag decision table for Task 1, with a SameSite rationale.
- A field classification for the ticket API response and one named field to remove or restrict.
- One chat-share sentence for the debrief.

---

## Key concepts

- **Cookie flags** — `HttpOnly`, `Secure`, and `SameSite` each address a specific attack vector; all three should be set in production.
- **Minimal responses** — return only what the client needs; every extra field is an exposure risk.
- **Token hygiene** — short expiry, no PII in payload, server-side signature verification on every request.
- **Log safety** — secrets and PII in logs are a breach waiting to happen.

---

## Next steps

Module 6 covers security testing — ZAP, Burp Suite, and SAST/DAST — which will surface the same categories of issue you just identified, but automatically and at scale.