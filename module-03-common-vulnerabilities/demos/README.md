# Module 3 — Demos

| Demo | Description |
|------|-------------|
| `00-setup-trustytickets.md` | Start the app, log in, and set up cookie jars for demos |
| `01-sqli-ticket-search.md` | SQL injection via `GET /api/tickets?search=` (bypass filters) |
| `02-idor-ticket-by-id.md` | IDOR via `GET /api/tickets/{id}` (access another user’s ticket) |
| `03-idor-attachment-download.md` | IDOR via `GET /api/attachments/{id}/download` (download another user’s file) |
| `04-stored-xss.md` | Stored XSS via comments + ticket description (innerHTML rendering) |

Add one `.md` per demo. Use **exercises/** for full lab write-ups (DVWA/WebGoat tasks).
