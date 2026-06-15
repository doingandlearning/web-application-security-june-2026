# Demo 04: JWT hygiene (decode + checklist)

Goal: cover JWT safely even if your course app uses cookie sessions.

## What you do live

1. Show a sample JWT (from your org, or a dummy token).
2. Paste into a decoder (e.g. jwt.io) or use a local decode tool.
3. Point out:
   - header (alg/kid)
   - payload (claims)
   - expiry (`exp`)

## What to say (script)

- “JWT is signed; it’s not encrypted by default.”
- “Anyone who has the token can read the payload.”
- “So we keep payload minimal: ids/roles only—no PII, no secrets.”
- “Short expiry for access tokens; refresh tokens are high value and should be rotated and revocable.”
- “Where you store it matters:
  - browser localStorage → XSS can steal it
  - HttpOnly cookie → harder to steal via JS, but CSRF becomes relevant”

## Quick checklist slide (verbal)

- **Verify** signature and expiry server-side
- **Expiry**: short-lived access tokens
- **Storage**: avoid JS-readable storage if possible
- **Revocation**: plan for it (refresh rotation, short expiry, allowlists/blacklists as needed)
- **No sensitive data** in claims

