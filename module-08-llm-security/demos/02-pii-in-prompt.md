# Demo 2: PII/Sensitive Data in Prompts (LLM06)

## Goal
Demonstrate why you should not paste real PII/secrets/proprietary code into public or ungoverned LLM tools, and how redaction/synthetic data changes the risk.

## Setup (use placeholders)
### Unsafe prompt (contains sensitive info)
```text
Here's our error log:
user_email: alice@example.com
client_ip: 203.0.113.55
api_key: sk_live_xxx_REDACT_ME

What caused the SQL timeout, and what should I do next?
```

### Safe prompt (same question, redacted/synthetic)
```text
Here's a generic (sanitised) error summary:
request_id: REQ_123
error_type: SQL timeout

What are common causes and safe next steps to investigate?
```

## Runbook: how to present it
1. Run the unsafe prompt first.
2. Ask learners: “What might go wrong if this prompt is logged or used for training?”
3. Run the safe prompt next.
4. Emphasize the boundary:
   - Redaction removes specific identifiers/secrets.
   - You still get guidance, but you reduce disclosure risk.

## Teaching tie-in
- OWASP LLM06: sensitive information disclosure
- Key habit: “When in doubt, don’t send” + redact or use synthetic data

## Suggested “what to say”
> “Your LLM is now a data access layer—so your inputs are the data it can accidentally expose.”

