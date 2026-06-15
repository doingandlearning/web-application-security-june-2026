# Demo 02: SameSite, CSRF, and “API thinking”

Goal: show **how CSRF works in a browser world**, where SameSite, CORS, and anti-forgery all play a part – and why “we only have APIs” is not a free pass.

---

## Step 1 — Ground the concept (1–2 minutes)

Say this:

- **CSRF in one sentence**: “The browser sends your cookies automatically, even if the request was triggered from another site.”
- If that cross-site request hits a **state-changing endpoint**, and the server only checks the cookie, you have CSRF.
- Defences:
  - **SameSite cookies** (browser-level mitigation),
  - **Anti-forgery tokens** (app-level mitigation),
  - **Sane CORS** when JSON APIs and JS are involved.

Tie it back to TrustyTickets:

- We have state-changing endpoints (create ticket, assign ticket, add comment).
- We use a cookie (`Session`) for auth.

---

## Step 2 — Show a state-changing endpoint (no attack yet)

In the browser:

1. Log in to TrustyTickets.
2. On `ticket.html`, add a comment or assign a ticket.
3. In DevTools → Network, highlight the request (e.g. `POST /api/tickets/{id}/comments` or `POST /api/tickets/{id}/assign`).

Point out:

- It’s **POST**, state-changing, and uses the `Session` cookie.
- The body is **JSON**, not form data.

Message: “This is exactly the kind of thing an attacker *wants* to trigger via the victim’s browser.”

---

## Step 3 — Run the “attacker page” with fetch

Open the attacker page from a different origin (e.g. `http://localhost:5500/index.html` if you’re serving it separately).

File: `module-05-session-management-data-protection/demos/index.html`

Key code:

```javascript
fetch('http://localhost:5000/api/tickets/1/assign', {
  method: 'POST',
  credentials: 'include', // ask browser to send cookies
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ assigneeId: 1 })
})
```

What happens in the browser:

- You click **“Send CSRF-ish request”**.
- DevTools console shows something like:
  - “CORS request did not succeed …”
- No change happens in TrustyTickets.

Say this:

- “The attacker JS is trying to:
  - send JSON,
  - include your cookie (`credentials: 'include'`),
  - and hit a state-changing endpoint.
  The browser is blocking it because **CORS and SameSite are currently tight**.”

---

## Step 4 — Use this to talk about APIs, not dismiss CSRF

Now pull out the key points you’re worried about:

- **“We just have APIs” is not enough.**
  - If your API is called from a browser and uses cookies for auth, CSRF is still a live issue.
  - Modern CSRF often looks like “malicious JS on another origin using fetch + misconfigured CORS,” not just a form post.

- **Two main gates in this demo:**
  - **SameSite**: controls when cookies are sent cross-site.
  - **CORS**: controls whether JS on another origin can successfully call your API and read the response.

Pose questions:

- “What happens if someone ‘temporarily’ adds a permissive CORS policy (e.g. `AllowAnyOrigin` + credentials)?”
- “Do you ever allow `SameSite=None` for SSO or cross-site flows? Is that scoped or global?”

---

## Step 5 — Bring anti-forgery back into the picture

Make the distinction:

- **SameSite/CORS**:
  - Browser-level controls.
  - Help reduce CSRF, especially accidental integration mistakes.
  - Can be bypassed or misconfigured.

- **Anti-forgery tokens**:
  - Application-level guarantee that the request came from your own front end.
  - Strong defence for high-value state changes (e.g. money transfer, role changes).

Line to use:

- “SameSite and CORS are great, but for sensitive operations you still want **an explicit token or double-submit pattern**, not just ‘hope CORS stays configured correctly forever’.”

---

## Key phrases to reinforce

- **“Browser auto-sends cookies”** – the root cause CSRF exploits.
- **“APIs + cookies + browsers still need CSRF thinking”** – it didn’t die with MVC forms.
- **“Modern CSRF = JS + misconfigured CORS + cookies”** – the shape of attacks has changed.

