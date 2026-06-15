---
title: "**Introduction to Application Security**"
sub_title: Module 1 — The Attack Surface
author: Kevin Cunningham
---

## A ticket in your system

A junior dev ships this endpoint:

```
GET /api/ticket?id=142
```

No authentication check. No ownership check. Returns the full ticket object.

<!-- pause -->

**Type in chat: bug / vulnerability / both / neither**

We'll come back to this at the end of the module.

<!-- speaker_note: Most will say "bug" or "both". Hold the tension — don't resolve it yet. The distinction between bug and vulnerability is the module's first real concept. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Where attacks touch your systems
===

<!-- end_slide -->

## The modern attack surface

Applications are the primary target — not firewalls, not the OS.

<!-- pause -->

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**Web**
Forms, cookies, headers, URLs

<!-- column: 1 -->

**Mobile**
APIs, device storage, tokens

<!-- column: 2 -->

**APIs**
JSON bodies, headers, partner integrations

<!-- reset_layout -->

<!-- pause -->

Attackers go where the value is: credentials, PII, business logic.

<!-- speaker_note: Ask "Which of these does your team ship?" — gets hands up or chat responses and anchors the rest of the module in their actual context. -->

<!-- end_slide -->

## Attack surface and trust boundaries

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

**Attack surface** — every place an attacker can send data or influence behaviour.

Examples: forms, query params, JSON bodies, headers, file uploads, webhooks.

<!-- pause -->

**Trust boundary** — where data crosses from untrusted to trusted.

Every trust boundary is a place to validate input and enforce policy.

<!-- column: 1 -->

<!-- new_lines: 2 -->

```
[Browser / Client]
       ↓
  ← trust boundary →
       ↓
  [Backend / API]
       ↓
  ← trust boundary →
       ↓
   [Database]
```

<!-- reset_layout -->

<!-- end_slide -->

## The gap most teams leave

**Type in chat: which does your team do most?**

<!-- incremental_lists: true -->

- Lock down infrastructure (firewalls, TLS, patching) and assume the app is safe
- Rely on client-side validation and "internal" API assumptions
- Treat security findings as the security team's job to fix

<!-- incremental_lists: false -->

<!-- pause -->

Small app-level mistakes cause the biggest incidents.

<!-- speaker_note: This is the disagreement beat — most teams will recognise themselves in option 1 or 3. Don't resolve it; the next few slides explain why each assumption breaks. -->

<!-- end_slide -->

<!-- jump_to_middle -->

How vulnerabilities happen
===

<!-- end_slide -->

## Vulnerabilities are usually unintentional

<!-- column_layout: [2, 3] -->

<!-- column: 0 -->

Most developers who introduce vulnerabilities are not making security mistakes — they're making normal coding decisions.

<!-- column: 1 -->

<!-- incremental_lists: true -->

- Missing validation — accepting whatever the client sends
- Copy-pasted code — string-built SQL, unsafe HTML rendering
- Assumptions — "who would call this API without auth?"
- Outdated dependencies — a known CVE in a library nobody's updated

<!-- reset_layout -->

<!-- pause -->

The same people who introduce issues are best placed to prevent and fix them.

<!-- end_slide -->

## Normal code, security impact

Three examples. Same line of reasoning each time.

<!-- pause -->

```
GET /api/ticket?id={id}   ← no ownership check
```
**Broken access control** — any user can read any ticket.

<!-- pause -->

```sql
"SELECT * FROM users WHERE name = '" + input + "'"
```
**SQL injection** — input becomes part of the query.

<!-- pause -->

```js
commentDiv.innerHTML = userComment
```
**Stored XSS** — comment renders as HTML, including scripts.

<!-- speaker_note: Each looks like a one-line feature decision. The point is not "these are bad" — it's that the security implication isn't obvious at the time you write it. -->

<!-- end_slide -->

## Exercise: What could go wrong?

```
GET /api/ticket?id=142
```

Returns: ticket title, description, assigned user, internal notes.

<!-- pause -->

**In pairs (3 minutes):** list what an attacker could *try* with this endpoint.

Not how — just what. Hypotheses only.

<!-- speaker_note: Listen for reading other users' tickets, enumerating IDs, accessing internal notes. Don't confirm or deny yet — save the "how" for Module 3. The goal is activating attacker thinking, not teaching exploitation. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Common threat families
===

<!-- end_slide -->

## Four families worth knowing now

<!-- incremental_lists: true -->

- **Injection** — user input ends up in queries, commands, or HTML
- **Broken access control** — no check that this user owns this resource (IDOR)
- **Auth and session flaws** — weak login, bad session handling, token leaks
- **Sensitive data exposure** — PII in logs, over-broad API responses, missing TLS

<!-- incremental_lists: false -->

<!-- pause -->

We'll exploit all four hands-on in Module 3.

<!-- speaker_note: Don't define each exhaustively — these are anchors, not a lecture. The exercise that follows will make them concrete. -->

<!-- end_slide -->

## Exercise: News story → threat family

I'll provide a recent security story (or use one you know).

**In breakout rooms (5 minutes):**

1. What was the impact — data loss, account takeover, outage, fraud?
2. Which of the four families fits best?
3. What single control would have reduced the risk most?

<!-- speaker_note: Pre-assign rooms. One rep per room reports back. Push back on vague answers "reduced risk" needs to be a specific control, not "better security". Common finding most incidents map to access control or injection, rarely to something exotic. -->

<!-- end_slide -->

## Attack vectors vs. threats

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Attack vector**
How an attacker reaches you.

Form, API endpoint, file upload, header, webhook.

<!-- column: 1 -->

**Threat**
What they're trying to do.

Steal data, escalate privilege, disrupt the service.

<!-- reset_layout -->

<!-- pause -->

Same threat, many vectors. Same vector, many threats.

<!-- pause -->

<!-- column_layout: [1, 3, 1] -->

<!-- column: 1 -->

**Rule of thumb:** internet-facing API handling PII is higher risk than an internal admin tool. Use both when prioritising.

<!-- reset_layout -->

<!-- end_slide -->

<!-- jump_to_middle -->

Web, mobile, and APIs
===

<!-- end_slide -->

## Same principles, different edges

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**Web**
XSS and CSRF are key concerns alongside injection and access control.

<!-- column: 1 -->

**Mobile**
Network APIs, local storage, logs, tokens stored on device.

<!-- column: 2 -->

**APIs**
Tokens and keys, rate limiting, over-broad responses, IDOR.

<!-- reset_layout -->

<!-- pause -->

Validate. Authenticate. Authorise. Encode. Protect data at rest and in transit.

The principles don't change — the attack surface does.

<!-- end_slide -->

## "Internal" doesn't mean safe

<!-- incremental_lists: true -->

- "It's only for our frontend" → exposed through a misconfigured gateway
- "It's behind VPN" → a compromised laptop or partner credential can still reach it
- "It's just a debug endpoint" → forgotten and left enabled in production

<!-- incremental_lists: false -->

<!-- pause -->

Design for **explicit trust boundaries**, not for assumptions that hold today.

<!-- speaker_note: Ask "Has anyone seen one of these in the wild?" Almost every team has a story. Let it breathe for 30 seconds. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Lab — Map Your Attack Surface
===

<!-- end_slide -->

## Lab: Map Your Attack Surface

Use TrustyTickets (or sketch your own app): browser → API → DB + external services.

**Your task:**

<!-- incremental_lists: true -->

1. Mark the **attack surface** — every input, call, upload, and header
2. Mark at least **two trust boundaries** and label what's untrusted at each side
3. At 2–3 key points, ask "what could go wrong?" and pick the highest-risk one

<!-- incremental_lists: false -->

<!-- pause -->

You'll reuse this map in Module 2 (threat modelling) and Module 3 (exploitation).

<!-- speaker_note: Circulate and prompt "Is that really a trust boundary, or just a network hop?" Push for specificity on the "what could go wrong" answer — a named threat, not just "it could be hacked". -->

<!-- end_slide -->

## Back to the ticket endpoint

```
GET /api/ticket?id=142
```

Earlier you said: **bug / vulnerability / both / neither**.

<!-- pause -->

It's a vulnerability — specifically **broken access control**.

Any authenticated user can read any ticket by changing the ID.

<!-- pause -->

**Type in chat: how would you fix it?**

<!-- speaker_note: Listen for check session user matches ticket owner, use opaque IDs (UUIDs), add authorisation middleware. All valid. The point is that the fix is one line of ownership logic — not infrastructure, not a firewall rule. -->

<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **Attack surface** — apps (web, mobile, APIs) are the primary target; every input is a potential entry point
2. **Trust boundaries** — where data moves from untrusted to trusted is where you validate and enforce policy
3. **Vulnerabilities are unintentional** — normal coding decisions with security implications
4. **Four threat families** — injection, broken access control, auth/session flaws, sensitive data exposure

<!-- end_slide -->

## Bridge to Module 2

**We've established:**

<!-- incremental_lists: true -->

- Where attacks touch your system
- What threat families to look for
- That small app-level decisions have the biggest impact

<!-- incremental_lists: false -->

**Module 2 — Secure SDLC:** where in your development lifecycle to think about this, and how to model threats before writing code.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===

*Introduction to Application Security — Module 1*