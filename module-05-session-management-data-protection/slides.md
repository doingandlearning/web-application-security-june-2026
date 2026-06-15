---
title: "**Introduction to Application Security**"
sub_title: Module 5 — Session Management and Data Protection
author: Kevin Cunningham
---

## A token in localStorage

Your team ships a React SPA. Authentication uses JWTs stored in `localStorage`.

The tokens expire after 24 hours. The app works. Users stay logged in.

A colleague raises it in code review. Your tech lead says "it's fine, we do input validation."

<!-- pause -->

**Type in chat: ship it / fix it first / it depends**

We'll come back to this at the end of the module.

<!-- speaker_note: Expect a split. "It depends" is the honest answer but push them — depends on what, exactly? The goal is to surface that localStorage and XSS interact in a specific, exploitable way that input validation does not address. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Session security
===

<!-- end_slide -->

## Cookie-based sessions

The browser holds a session cookie. The server uses it to identify the user.

Security depends on how that cookie is configured — and on assuming something will go wrong elsewhere.

<!-- pause -->

<!-- column_layout: [2, 3] -->

<!-- column: 0 -->

**Rule of thumb**

Assume XSS and CSRF exist somewhere in the app. Design sessions to reduce the blast radius if they do.

<!-- column: 1 -->

| Flag | What it does |
|---|---|
| `HttpOnly` | JavaScript cannot read the cookie |
| `Secure` | Cookie only sent over HTTPS |
| `SameSite` | Controls cross-site sending |

<!-- reset_layout -->

<!-- end_slide -->

## SameSite — the decision that matters

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**`Lax`**
Good default for most app sessions. Blocks cross-site POST; allows top-level navigation.

<!-- column: 1 -->

**`Strict`**
Maximum protection — but can break legitimate flows (OAuth redirects, links from email).

<!-- column: 2 -->

**`None`**
Required for cross-site flows. Must be paired with `Secure`. Needs explicit CSRF protections.

<!-- reset_layout -->

<!-- pause -->

**Type in chat — which would you use for a banking app / a public-facing blog / an OAuth provider?**

<!-- speaker_note: Three different contexts, three different answers. Banking — Strict or Lax. Blog — Lax. OAuth provider — None with Secure and CSRF tokens. The point is that there is no single right answer — it depends on the trust model. -->

<!-- end_slide -->

## Session lifecycle

Three decisions that most teams leave as defaults.

<!-- pause -->

<!-- incremental_lists: true -->

- **Regenerate the session ID after login** — prevents session fixation; a pre-login session ID should not survive authentication
- **Idle timeout** — sensitive apps need short timeouts; "never expire" is not a session policy
- **Logout behaviour** — does logout actually invalidate the session server-side, or just delete the cookie client-side?

<!-- incremental_lists: false -->

<!-- pause -->

**Type in chat — does your app's logout actually invalidate the session?**

<!-- speaker_note: Many teams will be unsure. This is a common gap — the cookie is cleared but the server-side session token is still valid. Ask one person to explain how they would check. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Tokens and JWTs
===

<!-- end_slide -->

## Cookies vs tokens — the real trade-off

The question is not which is better. It is which failure mode you are more prepared to handle.

<!-- pause -->

| | Cookie session | Token / JWT |
|---|---|---|
| State | Server-side (easy to revoke) | Stateless (harder to revoke) |
| Revocation | Server invalidates the session | Needs expiry or a blacklist |
| XSS risk | Low if HttpOnly | High if stored in localStorage |
| CSRF risk | Present — mitigate with SameSite | Lower for non-cookie storage |

<!-- pause -->

**Type in chat — which failure mode is harder to recover from: session hijack via CSRF, or token theft via XSS?**

<!-- speaker_note: No correct answer. Token theft via XSS is often worse because the attacker has the credential itself, not just a session. CSRF is scoped to what the browser will do on the victim's behalf. Push for reasoning, not just a choice. -->

<!-- end_slide -->

## JWT — what it is and what it is not

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**What a JWT is**

A signed token — header, payload, signature, base64-encoded.

The signature proves it has not been tampered with.

The payload is **not encrypted** — anyone with the token can read it.

<!-- column: 1 -->

**What a JWT is not**

Not a secret store — never put passwords, keys, or PII in the payload.

Not inherently secure — security depends entirely on signature verification, expiry enforcement, and safe storage.

<!-- reset_layout -->

<!-- end_slide -->

## JWT checklist

<!-- incremental_lists: true -->

- Short expiry on access tokens — minutes to a few hours; not 24 hours, not 30 days
- Refresh tokens — rotate on use; revoke on logout; treat as high-value credentials
- Payload — identifiers only; no secrets, no PII
- Signature verification — always server-side, on every request; never trust an unverified token
- Storage — see next slide

<!-- incremental_lists: false -->

<!-- end_slide -->

## Token storage — the trade-off

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**`localStorage` or JS memory**

Easy to implement.

If the app has any XSS vulnerability, an attacker script can read and exfiltrate the token.

Input validation does not fully protect this.

<!-- column: 1 -->

**HttpOnly cookie**

JavaScript cannot read it — XSS cannot steal it directly.

Needs SameSite and CSRF mitigations for cross-site requests.

More setup; harder to get wrong under XSS.

<!-- reset_layout -->

<!-- pause -->

Mobile: use platform secure storage — Keychain on iOS, Keystore on Android.

<!-- end_slide -->

<!-- jump_to_middle -->

Data protection
===

<!-- end_slide -->

## Data in transit and at rest

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**In transit**

HTTPS everywhere — including internal service-to-service traffic.

HSTS enforces HTTPS in browsers and prevents downgrade attacks.

Do not assume the internal network is trusted.

<!-- column: 1 -->

**At rest**

Encrypt sensitive data — at the database, volume, or field level depending on sensitivity.

The algorithm matters less than key management.

Keys belong in a vault or managed service — not in source code, not in environment variables committed to the repo.

<!-- reset_layout -->

<!-- end_slide -->

## Self-inflicted leaks — logs and errors

These are not exotic attack vectors. They are routine findings in every pen test.

<!-- pause -->

<!-- incremental_lists: true -->

- Passwords, tokens, and session IDs in application logs
- PII — email addresses, identifiers, health or financial data — in debug output
- Verbose error messages returned to the client exposing stack traces, table names, or query structure

<!-- incremental_lists: false -->

<!-- pause -->

**Rule:** client-facing errors are generic. Detailed errors go to server-side logs only — and those logs are access-controlled.

<!-- speaker_note: Ask "has anyone seen a stack trace returned in a production API response?" — almost everyone has. It is a reliable way to get the room's attention before the API section. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Secure API design
===

<!-- end_slide -->

## APIs as trust boundaries

Every API endpoint that touches sensitive data or state needs the same treatment as any other trust boundary.

<!-- incremental_lists: true -->

- **Authenticate every request** — not just the ones that feel sensitive
- **Authorise per resource** — ownership and role checks at the endpoint, not just at login
- **Return minimal fields** — DTOs, not entire object graphs; the caller gets what they need
- **Rate limiting and payload limits** — protection against brute force and abuse

<!-- incremental_lists: false -->

<!-- end_slide -->

## Common API abuse patterns

Four patterns that appear repeatedly in pen test findings.

<!-- pause -->

**Enumeration** — IDOR plus sequential IDs. We exploited this in Module 3. Rate limiting and opaque IDs reduce the window.

<!-- pause -->

**Brute force** — login and password reset endpoints without rate limiting or lockout. One of the easiest findings to prevent.

<!-- pause -->

**Mass assignment** — a client sends extra fields and the server binds them all to the model. An attacker sets `isAdmin: true`.

<!-- pause -->

**Verbose errors** — stack traces and query details in error responses. Covered above — worth repeating because it appears in almost every engagement.

<!-- end_slide -->

## Exercise — session and token audit

Pick one of the following for your app or TrustyTickets.

**Option A — Cookie hardening**
Check the session cookie in devtools. Are HttpOnly, Secure, and SameSite set? What happens at logout — does the server invalidate the session?

**Option B — JWT review**
Decode a token. What is in the payload? What is the expiry? Where is it stored? Does the server verify the signature on every request?

**Option C — Log and error audit**
Trigger an error. What does the response contain? Search the server logs for passwords, tokens, or session IDs.

<!-- pause -->

**In pairs (8 minutes):** pick one option, investigate, and bring back one finding and one fix.

<!-- speaker_note: Circulate and prompt. Option A tends to surface the logout gap. Option B tends to surface long expiry or PII in the payload. Option C almost always produces a finding. Push pairs to write the fix as a specific code change, not a vague recommendation. -->

<!-- end_slide -->

## Back to the React SPA

JWTs in `localStorage`. 24-hour expiry. Input validation in place.

You said: **ship it / fix it first / it depends**.

<!-- pause -->

The tech lead is right that input validation helps. But it does not eliminate XSS risk — one missed output encoding point, one third-party script, one dependency vulnerability.

<!-- pause -->

If XSS runs, `localStorage` is readable. The token is gone. The attacker has 24 hours.

<!-- pause -->

The fix is not complex — HttpOnly cookie storage, shorter expiry, refresh token rotation.

**The question was never "does it work?" It was "what is the blast radius when something else goes wrong?"**

<!-- speaker_note: Return to the chat poll. If anyone said "it depends" — this is what it depends on. The XSS surface of the application and the value of the data being protected. A low-sensitivity internal tool is a different decision to a payments app. -->

<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **Cookies** — HttpOnly, Secure, SameSite; regenerate after login; verify logout actually invalidates server-side
2. **JWTs** — short expiry, identifiers only in payload, signature verified server-side, stored in HttpOnly cookies not localStorage
3. **Data protection** — HTTPS everywhere including internal traffic; keys in a vault; generic client errors, detailed server logs
4. **APIs** — authenticate and authorise every request; minimal responses; rate limiting on abuse-prone endpoints

<!-- end_slide -->

## Bridge to Module 6

**We've established:**

<!-- incremental_lists: true -->

- How to configure sessions and tokens to limit blast radius
- Where data leaks through logs, errors, and over-broad API responses
- The trade-offs between storage options and session models

<!-- incremental_lists: false -->

**Module 6 — Security testing:** ZAP and Burp Suite to find what we missed; SAST and DAST in the pipeline.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===

*Introduction to Application Security — Module 5*