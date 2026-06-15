# Demo 04: Stored XSS — comments and description (innerHTML)

Goal: show how “just displaying user content” becomes stored XSS when the app renders untrusted input as HTML.

## Prereqs

- TrustyTickets running (see `00-setup-trustytickets.md`)
- You have `alice.cookies` (logged in as Alice)

```bash
export TT_BASE="http://localhost:5005"
```

---

## Option A (most reliable live): stored XSS in comments (UI)

This avoids shell/JSON escaping issues. It’s the best live demo.

### 1) Add an XSS payload directly in the comment box

1. Log in as **Alice**
2. Open any ticket (e.g. `ticket.html?id=1`)
3. Paste this into the “Add a comment…” box:

```html
<img src=x onerror=alert(1)>
```

4. Click **Add comment**

### 2) Trigger the XSS

Reload the page (or view the ticket as another user).

Expected: an alert fires when comments render.

If you want an even more visible effect than an alert, use:

```html
<img src=x onerror="document.body.style.background='red'">
```

Talking points:

- This is **stored**: the payload is saved and executes for anyone who views the ticket.
- The root cause is rendering untrusted data with `innerHTML`.

---

## Option B: stored XSS in ticket description (UI)

### 1) Create a ticket whose description is HTML/JS

Create a new ticket and paste an HTML payload into the **Description** prompt:

```html
<img src=x onerror=alert(1)>
```

Note the returned `id` (e.g. `3`).

### 2) Trigger the XSS in the browser

Open:

```text
http://localhost:5005/ticket.html?id=<id>
```

Expected: the alert fires when the ticket loads (description is rendered).

Talking points:

- This is **stored**: the payload lives in the database.
- Anyone who views the ticket will execute the attacker’s code in their browser.

---

---

## Option C (copy/paste commands): stored XSS via API (curl)

If you want to drive this via `curl`, keep the payload simple so you don’t fight quoting.

### Add a comment payload via API

```bash
curl -s -b alice.cookies -H "Content-Type: application/json" \
  -d '{"body":"<img src=x onerror=alert(5)>"}' \
  "$TT_BASE/api/tickets/1/comments"
```

Then open:

```text
http://localhost:5005/ticket.html?id=1
```

If your DB isn’t freshly seeded and ticket `1` doesn’t exist, pick any ticket id you can open in the UI and use that id in the URL and API path.

## What to explain (say this)

- The app uses `innerHTML` to render untrusted content (comments and description).
- If you need rich text, you still must:
  - encode by default, and
  - use strict sanitisation/allowlists when you truly need HTML.

Key takeaway:

- Fix is **safe rendering** (`textContent`, templating with auto‑escape, context‑aware encoding, CSP) — Module 4/5.

