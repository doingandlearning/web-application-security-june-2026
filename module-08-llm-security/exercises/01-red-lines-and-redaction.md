# Lab 1: Red lines and redaction (LLM06)

## Objective
Practice “when in doubt, don’t send” by rewriting sensitive support/debug text so it is safe to paste into a public or ungoverned LLM.

## Scenario
You are asked (by a teammate) to “paste this into ChatGPT to debug.” The text below contains sensitive data.

## Task
1. Identify all sensitive categories present (PII, secrets, proprietary code/internal identifiers).
2. Rewrite the message so it is safe to paste:
   - redact specific identifiers (emails, IPs, names, user IDs)
   - remove secrets/credentials
   - avoid proprietary/internal-only code or endpoints
   - keep enough context for helpful debugging
3. Provide the “safe version” and list 2 things you would verify internally before sending again.

## Given (unsafe support request)
```text
Hi! Our login endpoint is failing.
User: bob.smith@company.com
Client IP: 203.0.113.55
Request id: REQ_98912
We think the issue is with our JWT auth middleware.
Here is a snippet:
// API endpoint:
/api/auth/callback
// (redacted for now) code references our internal signing key path:
SIGNING_KEY_PATH=/vault/prod/signing-key
Error:
Invalid token: signature verification failed
```

## Hints
- If you wouldn't post it to the public internet, assume an ungoverned LLM could expose it.
- Replace with synthetic placeholders like `user@example.com`, `REQ_###`, and generic endpoint names.

<details>
<summary>Suggested solution approach (not a single “correct answer”)</summary>

```text
Rewrite as:
- Remove real email and IP.
- Remove internal vault path / secrets.
- Keep: the generic error, what component fails (JWT middleware), and request id as a synthetic value.
```

</details>

