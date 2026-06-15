# Module 3 — Exercises

## Lab 3: Exploit common vulnerabilities

This lab has four tasks. Tasks 1–3 are the core exploitation exercises. Task 4 (one-line fixes) is a short debrief that runs immediately after and bridges to Module 4 — do not skip it.

Allow 45–60 minutes for all four tasks. If time is tight, complete at least Tasks 1 and 2 before moving to Task 4.

---

## Scenario: TrustyTickets

You are working in an isolated lab environment running TrustyTickets — an intentionally vulnerable web application. It contains real data and test user accounts (`alice`, `bob`). Nothing here is production. You are explicitly authorised to test it.

**Rule:** break only what you own or are explicitly authorised to test. Techniques practised here are for authorised testing only.

Start the app: `cd TrustyTickets && dotnet run`

Log in as `alice` before starting Task 1.

---

## Task 1: SQL injection (10–15 minutes)

### Goal

Use a crafted input to retrieve data you should not be able to see as a normal user.

### Your task

Navigate to the **Reports** page (`reports.html`). Use the assignee search field.

Start with a simple test to confirm the field is vulnerable:

```
'
```

Observe whether you get a SQL error or unexpected behaviour. Then try a payload that returns more rows than expected:

```
%' ) OR 1=1 -- 
```

Confirm that results appear which should not be visible to your user.

**Teaching route** (if you want to see the simpler version first): use the search box on `tickets.html` — `GET /api/tickets?search=...`

**Lab route**: use the assignee search on `reports.html` — `GET /api/reports/assignees?q=...`

**Hints:**
- Look for a change in the number of results — more rows than expected is the signal.
- The `--` at the end comments out the rest of the original SQL query.
- If the first payload produces an error, the field is almost certainly vulnerable — try a variation.

<details>
<summary>Possible solution</summary>

```
Input:   %' ) OR 1=1 -- 
Result:  the query returns all assignees, not just those matching the search term.
Why:     the input is concatenated into the SQL query; OR 1=1 makes the WHERE clause always true.
Pattern: string concatenation of untrusted input into SQL.
```

</details>

---

## Task 2: Broken access control / IDOR (10 minutes)

### Goal

Access another user's resource by changing an identifier in the request.

### Your task

View one of your own tickets as `alice`. Note the ticket ID in the URL or request.

Using the browser address bar or browser dev tools network tab, change the ID to a value belonging to `bob`:

```
GET /api/tickets/{id}        ← teaching route
GET /api/attachments/{id}/download   ← lab route
```

Observe whether you can see `bob`'s data.

**Hints:**
- IDs in TrustyTickets are sequential integers — try incrementing or decrementing by one.
- The network tab in browser dev tools shows the raw request and response without needing a proxy.
- Try both the ticket endpoint and the attachment download endpoint.

<details>
<summary>Possible solution</summary>

```
Request: GET /api/tickets/101   (alice's ticket)
Change:  GET /api/tickets/102   (bob's ticket)
Result:  bob's ticket details are returned in full, including internal notes.
Why:     the server fetches the ticket by ID without checking that the requesting user owns it.
Pattern: missing ownership check before returning a resource.
```

</details>

---

## Task 3: Stored XSS (10 minutes)

### Goal

Inject a payload that executes in another user's browser when they view the page.

### Your task

First, submit a harmless test value in the comment box on any ticket to confirm where it appears:

```
TEST123
```

Reload the page and confirm `TEST123` renders as visible text. Then replace it with a payload:

```html
<img src=x onerror=alert(1)>
```

Reload the page. If the payload executes, an alert box appears. This confirms the input is rendered as HTML rather than escaped as text.

**Teaching route**: comment box on `ticket.html`

**Lab route**: ticket description field — create a new ticket with HTML in the description

**Hints:**
- If `<script>alert(1)</script>` does not fire, try the `<img>` payload above — it is more reliable across browsers.
- Look for any field where HTML tags render as formatting rather than as literal text.

<details>
<summary>Possible solution</summary>

```
Input:   <img src=x onerror=alert(1)>  in the comment box
Result:  an alert fires for every user who views that ticket
Why:     the comment is passed to innerHTML, so the browser treats it as HTML and executes the handler
Pattern: untrusted input rendered as HTML/JS rather than as plain text
```

</details>

---

## Stretch: find one more route

If you finish Tasks 1–3 early, pick one vulnerability type and find a second instance of it in TrustyTickets:

- Another input that reaches a SQL query (SQLi-style)
- Another endpoint that fetches a resource by ID without an ownership check (IDOR-style)
- Another field where untrusted content is rendered as HTML (XSS-style)

Write down the route, the payload or change you made, and the result.

---

## Task 4: One-line fixes (10 minutes)

Run this immediately after Tasks 1–3, before moving to Module 4.

### Step 1: Individual (3 minutes)

For each exploit you completed, write exactly one sentence describing the fix. Use the pattern "To fix this, I would…"

Examples of what a specific sentence looks like:

- "To fix the SQLi, I would replace string concatenation with a parameterised query or ORM `.Where()` call."
- "To fix the IDOR, I would add an ownership check before returning the ticket — compare `ticket.OwnerId` to the current user's ID."
- "To fix the XSS, I would replace `innerHTML` with `textContent` so the input renders as plain text."

Vague answers to avoid: "validate the input", "sanitise it", "add security checks".

### Step 2: Chat share

Type your fix for Task 1 (SQLi) in chat. One sentence only.

The facilitator will read a few responses and group them. The pattern that emerges — parameterisation, ownership checks, safe rendering — is what Module 4 covers in detail.

---

## Expected output

- Notes confirming each exploit succeeded: the endpoint used, the payload or change made, and what was returned.
- One sentence per vulnerability: the coding pattern that made it possible.
- One sentence per vulnerability: the specific fix.

---

## Key concepts

- **Injection** — untrusted input ends up in a query or command; parameterisation prevents it.
- **Broken access control** — missing ownership check; one server-side condition prevents it.
- **Stored XSS** — untrusted input rendered as HTML; safe DOM APIs prevent it.
- **Exploit to fix** — every attack maps directly to a coding pattern, and every coding pattern has a specific fix.

---

## Next steps

Module 4 applies the fixes you just described — parameterised queries, output encoding, and ownership checks — to the same TrustyTickets codebase. Keep your notes. You will rerun these payloads after fixing to confirm they no longer work.