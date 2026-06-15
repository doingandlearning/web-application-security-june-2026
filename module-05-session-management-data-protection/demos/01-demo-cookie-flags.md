# Demo 01: Cookie flags (HttpOnly / Secure / SameSite)

Goal: make cookie flags concrete and tie them to XSS/CSRF risk.

## What you’ll show

1. In DevTools → Cookies, inspect the `Session` cookie.
2. In code, show where it’s configured.

## Where it is in code

- `TrustyTickets/Controllers/AuthController.cs`

Look for:

- `HttpOnly = true`
- `SameSite = SameSiteMode.Lax`
- `Secure = ...` (currently not set; discuss)
- `MaxAge = ...`

## Talking points (use as a script)

- **HttpOnly**:
  - “If an attacker gets XSS, they often try to steal the session cookie.”
  - “HttpOnly blocks JavaScript access to the cookie.”
  - “It doesn’t *prevent* XSS; it reduces impact.”

- **Secure**:
  - “Cookie only sent over HTTPS.”
  - “On localhost HTTP demos, setting Secure can stop your cookie being sent at all—so in training we may keep HTTP, but in real deployments HTTPS is non-negotiable.”

- **SameSite**:
  - “Controls whether the browser sends cookies on cross-site requests.”
  - “`Lax` is a common default; it reduces CSRF in many common cases.”

## Optional “show the headers” (curl)

```bash
curl -i -H "Content-Type: application/json" \
  -d '{"userName":"alice","password":"alice123"}' \
  http://localhost:5000/api/auth/login
```

Look for the `Set-Cookie:` header and point out attributes.

---

## If they say “we already know this”

Use these to deepen the conversation without getting stuck on basics:

- **“When do you *not* want Strict SameSite?”**
  - SSO / cross-site POST flows, third-party IdPs, some legacy integrations.
  - Follow-up: “How do you scope that exception so it’s not global?”

- **“How do you rotate session secrets / keys?”**
  - Are encryption/signing keys for cookies or session stores ever rotated?
  - What happens to existing sessions when you do?

- **“What’s your policy on session lifetime?”**
  - Idle timeout vs absolute lifetime.
  - Do “remember me” sessions exist? How long? Where is that policy written down?

- **“What happens if someone logs in from a new device?”**
  - Do you invalidate other sessions? Show a device list? Do nothing?
  - Where is that behaviour defined (security vs product vs “we inherited it”)?

- **“Where do cookie settings live in your codebase?”**
  - Scattered per-controller, or centralised in middleware/options?
  - How hard would it be to audit all cookies for missing flags?

These questions turn “yes, we set HttpOnly” into a richer discussion about **lifetime, rotation, exceptions, and visibility**, without requiring you to know every C# detail. 

