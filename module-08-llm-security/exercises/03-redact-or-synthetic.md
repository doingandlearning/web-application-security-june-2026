# Lab 3: Redact or synthetic (LLM06)

## Objective
Practice rewriting a “support request” into something safe to paste into a public or ungoverned LLM.

## Scenario
You need help debugging a production incident. You must not leak sensitive data.

## Task
1. Classify the sensitive items in the message.
2. Rewrite the request in two variants:
   - Redacted: keep the same structure, but remove/replace identifiers and secrets.
   - Synthetic: replace details with realistic but fake values.
3. Explain one rule you used: “What should we never send?” and “What can we send safely?”

## Given (unsafe support message)
```text
Our database is returning:
ERROR: duplicate key value violates unique constraint "users_email_key"
Request correlation id: REQ_42001
User: carol@company.com
Client IP: 198.51.100.77
Stack trace (excerpt):
at AuthMiddleware.Validate(token) in /src/auth/secret_key_loader.ts

Can you explain why this happens and how to fix it?
```

## Hints
- Replace emails with `user@example.com`-style placeholders.
- Replace correlation IDs with `REQ_###`.
- Replace internal file paths with generic module names.
- If there are secrets, remove them entirely.

<details>
<summary>Suggested solution direction</summary>

```text
Redacted example:
- User: [REDACTED_EMAIL]
- Client IP: [REDACTED_IP]
- Stack excerpt: show only the function name, not internal paths.
Synthetic example:
- User: user@example.com
- Client IP: 203.0.113.10
- correlation id: REQ_123
```

</details>

