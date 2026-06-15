---
title: "**Introduction to Application Security**"
sub_title: Module 3 — Common Vulnerabilities
author: Kevin Cunningham
---

## You find this in a PR review

```js
const results = await db.query(
  "SELECT * FROM tickets WHERE title LIKE '%" + search + "%'"
);
```

A colleague added it yesterday. The feature works. Tests pass.

<!-- pause -->

**Type in chat: approve / request changes / it depends**

We'll come back to this at the end of the module.

<!-- speaker_note: Most will say "request changes" or "it depends" — push them to say what they'd write in the review comment. The point is that recognising a vulnerability in code review is different from knowing the name of it. -->

<!-- end_slide -->

<!-- jump_to_middle -->

The OWASP Top 10
===

<!-- end_slide -->

## The risks that matter most

OWASP Top 10 — the most critical web application security risks, ranked by prevalence and impact.

<!-- pause -->

This module focuses on three attack types across two OWASP categories:

<!-- pause -->

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**SQL injection**
(A03:2025 — Injection)

Untrusted input ends up in a database query.

<!-- column: 1 -->

**Broken access control**
(A01:2025 — Broken Access Control)

No check that this user owns this resource.

<!-- column: 2 -->

**Cross-site scripting (XSS)**
(A03:2025 — Injection)

Untrusted input rendered as HTML or JavaScript in a victim's browser.

<!-- reset_layout -->

<!-- pause -->

OWASP groups SQLi and XSS together because the root cause is the same — untrusted input treated as code. They are worth understanding separately because the fix, the context, and the impact differ.

<!-- speaker_note: The closing line heads off the question "why are two of these the same category?" The answer is that OWASP cares about root cause; we care about attack surface and fix pattern. Both framings are useful. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Injection
===

<!-- end_slide -->

## SQL injection — how it happens

Untrusted input concatenated directly into a SQL query.

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Vulnerable**

```sql
SELECT * FROM tickets
WHERE title LIKE '%{search}%'
```

If `search` is `' OR '1'='1` — the query returns every row.

<!-- column: 1 -->

**What an attacker can do**

- Read data they should not see
- Bypass authentication
- In some databases — write, delete, or execute commands

<!-- reset_layout -->

<!-- pause -->

Impact ranges from data exposure to full database compromise.

<!-- speaker_note: Draw attention to the fact that the vulnerable pattern looks exactly like the PR in the opening scenario. Don't confirm the answer yet — that comes at the close. -->

<!-- end_slide -->

## Demo — SQL injection

**Demo:** open TrustyTickets search. Enter a standard query, then a crafted payload.

Show what comes back.

<!-- speaker_note: Run this live. Use the search endpoint — try a normal search first so the expected behaviour is clear. Then use a payload that returns extra rows. Ask "what changed?" before explaining why it worked. If the demo breaks, narrate what would have happened — the mechanism matters more than a clean run. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Broken access control
===

<!-- end_slide -->

## IDOR — how it happens

The endpoint uses an ID from the request but never checks ownership.

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**The request**

```http
GET /api/tickets/123
```

Returns ticket 123 — title, description, internal notes, assigned user.

<!-- column: 1 -->

**The question the code never asks**

Does the user making this request own ticket 123?

```js
// what is missing
const ticket = await Ticket.findById(id);
// no: if (ticket.userId !== req.user.id)
return ticket;
```

<!-- reset_layout -->

<!-- pause -->

Change `123` to `124`. If you get a result — that is a broken access control vulnerability.

<!-- end_slide -->

## Demo — IDOR

**Demo:** log in as User A. Fetch a ticket. Change the ID to a ticket owned by User B.

Show what the API returns.

<!-- speaker_note: Run this live. The contrast between what the UI shows and what the raw API returns is often the most striking moment — learners realise the frontend is not a security control. Ask "why did that work?" before explaining the missing ownership check. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Cross-site scripting
===

<!-- end_slide -->

## XSS — how it happens

Untrusted input rendered as HTML or JavaScript — usually via `innerHTML` or an unescaped template.

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Vulnerable**

```js
commentDiv.innerHTML = userComment;
```

If `userComment` contains `<script>alert(1)</script>` — that script runs in every visitor's browser.

<!-- column: 1 -->

**Stored XSS**

Payload saved in the database. Runs for every user who views the page — not just the attacker.

Impact: session theft, keylogging, defacement, phishing — all without any interaction from the victim beyond page load.

<!-- reset_layout -->

<!-- end_slide -->

## Demo — stored XSS

**Demo:** submit a comment containing a script tag. Reload the page as a different user.

Show the script executing.

<!-- speaker_note: Run this live. Use a simple alert payload first — the visual confirmation is immediate. Then show what a real payload could do (steal document.cookie) without actually exfiltrating anything. Ask "who is the victim here?" — the answer is not the attacker, it is every subsequent user who loads the page. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Lab — exploit common vulnerabilities
===

<!-- end_slide -->

## Lab setup

You are working in an isolated environment — DVWA, WebGoat, or TrustyTickets. No real data. No real users.

**Rule:** break only what you own or are explicitly authorised to test. Techniques practised here are for authorised testing only.

<!-- pause -->

Work in pairs. Use the task sheet in `exercises/` for each target, hints, and optional solution references.

<!-- end_slide -->

## Lab targets

Complete as many as time allows — in this order.

<!-- pause -->

**Target 1 — SQL injection**
Use a crafted search payload to read data you should not have access to.

<!-- pause -->

**Target 2 — IDOR**
Access another user's ticket record by changing an ID in the request.

<!-- pause -->

**Target 3 — Stored XSS**
Inject a payload that executes when another user loads the page.

<!-- speaker_note: Circulate and prompt pairs who are stuck. Common sticking point on SQLi — they try complex payloads before a simple one. On IDOR — remind them to use the browser dev tools network tab, not just the UI. On XSS — angle brackets are often stripped by the UI input; try the API directly. -->

<!-- end_slide -->

## After the lab — connect to code

For at least one exploit you completed:

<!-- pause -->

1. Find the line of code that made it possible
2. Write one sentence — "To fix this, I would …"

<!-- pause -->

Examples of what that sentence looks like:

- "To fix the SQLi, I would use a parameterised query instead of string concatenation."
- "To fix the IDOR, I would add an ownership check before returning the ticket."
- "To fix the XSS, I would use `textContent` instead of `innerHTML`."

<!-- pause -->

Share one with the group. We will turn these into the full patterns in Module 4.

<!-- speaker_note: The one-sentence fix is deliberately constrained. Push back on vague answers like "validate the input" — ask what specifically they would change in the code they saw. -->

<!-- end_slide -->

## Back to the PR

```js
const results = await db.query(
  "SELECT * FROM tickets WHERE title LIKE '%" + search + "%'"
);
```

You said: **approve / request changes / it depends**.

<!-- pause -->

This is SQL injection. String concatenation, untrusted input, no parameterisation.

<!-- pause -->

The review comment is not "this is dangerous" — it is:

**"Use a parameterised query. Replace with `db.query('... LIKE ?', ['%' + search + '%'])`."**

<!-- speaker_note: Return to the chat poll answers. If anyone said "approve" — they now know why that was wrong. The point is that recognising and naming the fix in a review comment is the skill, not just flagging concern. -->

<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **Injection** — string concatenation of untrusted input into queries or commands; fix with parameterised queries
2. **Broken access control** — missing ownership checks on IDs; fix with server-side authorisation at every endpoint
3. **XSS** — untrusted input rendered as HTML; fix with output encoding and safe DOM APIs
4. **Same root causes** — concatenation, missing checks, unsafe output — across different vulnerability types

<!-- end_slide -->

## Bridge to Module 4

**We've established:**

<!-- incremental_lists: true -->

- What these vulnerabilities look like in running code and in a request
- How to exploit them in a controlled environment
- The one-line fix for each

<!-- incremental_lists: false -->

**Module 4 — Input validation and secure coding:** parameterised queries, output encoding, and secure authentication patterns — applied to the same codebase you just broke.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===

*Introduction to Application Security — Module 3*