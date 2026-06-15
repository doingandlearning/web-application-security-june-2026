# Module 8 — Exercises

## Lab 8: LLM security in practice

This lab has three required tasks and two extension tasks. Tasks 1, 2, and 3 cover the core skills — redacting before you send, verifying security-critical output, and reviewing Copilot suggestions. Tasks 4 and 5 go deeper on hallucination and data governance.

Allow 40–50 minutes for Tasks 1–3 plus debriefs.

---

## Task 1: Redact or synthetic — safe prompting (10–12 minutes)

**OWASP LLM06 — Sensitive information disclosure**

### Scenario

You need help debugging a production incident and want to paste the details into a public LLM. You must not leak sensitive data, but you also need to keep enough context for a useful answer.

### Step 1: Classify the sensitive items

Read the message below. List every sensitive item and classify it — PII, secret or credential, internal identifier, or proprietary path.

```
Our database is returning:
ERROR: duplicate key value violates unique constraint "users_email_key"
Request correlation id: REQ_42001
User: carol@company.com
Client IP: 198.51.100.77
Stack trace (excerpt):
  at AuthMiddleware.Validate(token) in /src/auth/secret_key_loader.ts

Can you explain why this happens and how to fix it?
```

### Step 2: Rewrite in two variants

**Redacted version** — keep the same structure, replace or remove sensitive items with placeholders:
- Emails → `[REDACTED_EMAIL]`
- IPs → `[REDACTED_IP]`
- Internal paths → generic module name
- Correlation IDs → `REQ_###`

**Synthetic version** — replace with realistic but entirely fake values:
- Email → `user@example.com`
- IP → `203.0.113.10`
- Correlation ID → `REQ_123`
- Path → `at AuthMiddleware.Validate(token)`

### Step 3: Write one rule

Complete this sentence: "Before pasting anything into a public or ungoverned LLM, we always remove ________."

**Hints:**
- If you would not post it to the public internet, assume an ungoverned LLM could expose it.
- The synthetic version is often more useful than the redacted version — it gives the model plausible context without revealing real data.
- Keep the error message and component name — those are safe and necessary for a useful answer.

<details>
<summary>Possible solution direction</summary>

```
Sensitive items: email (PII), IP (PII), correlation ID (internal identifier), file path (proprietary internal structure)

Redacted:
  User: [REDACTED_EMAIL]
  Client IP: [REDACTED_IP]
  Request id: REQ_###
  Stack: at AuthMiddleware.Validate(token)

Synthetic:
  User: user@example.com
  Client IP: 203.0.113.10
  Request id: REQ_123
  Stack: at AuthMiddleware.Validate(token)

Rule: "Before pasting into a public LLM, we always remove real emails, IPs,
      internal paths, credentials, and customer identifiers."
```

</details>

### Debrief (chat share)

Type in chat: **your one "never send" rule — one sentence.**

The facilitator will read a few. Common gap: teams forget that correlation IDs and internal file paths are also sensitive — they reveal system structure that helps an attacker.

---

## Task 2: Verify a JWT validation function (12–15 minutes)

**OWASP LLM02 + LLM09 — Insecure output handling and overreliance**

### Scenario

A teammate pastes this into a PR — "Copilot suggested it, looks right to me."

```js
// WARNING: example only — intentionally flawed
function validateJwt(token) {
  const payload = JSON.parse(atob(token.split('.')[1]));
  // If header.alg is missing or none, still accepts.
  // If exp is missing, still accepts.
  return true;
}
```

Your job is to list what must be verified before this merges.

### Step 1: List at least five verification points

For each point, state:
- What could be wrong
- What you would check — docs, unit tests, config, or existing auth library patterns

### Step 2: Write the PR comment

Write the comment you would leave on this PR. Be specific — name the check and why it matters. "This needs review" is not a PR comment.

**Hints:**
- Separate "looks correct" from "is correct."
- Anything touching auth, crypto, or tokens must be verified regardless of source.
- The function always returns `true` — that alone should stop the review.

<details>
<summary>Verification points</summary>

```
1. Signature verification — the function decodes the payload but never verifies the signature.
   A tampered token will pass. Check: does it call a verify() function with the signing key?

2. Algorithm acceptance — the alg: none attack allows an unsigned token to pass.
   Check: does it explicitly reject alg: none and enforce the expected algorithm?

3. Expiry check — exp is not checked. An expired token will pass indefinitely.
   Check: does it compare exp to the current timestamp and reject expired tokens?

4. Error handling — malformed tokens cause JSON.parse to throw; there is no try/catch.
   The function should fail closed (return false / throw) on any parse error, not silently pass.

5. Unit tests — add tests for: expired token, wrong signature, alg: none token,
   malformed token, missing exp claim. All should be rejected.
```

</details>

### Debrief (chat share)

Type in chat: **the one check you would call out first in the PR comment — one sentence.**

The facilitator will read responses. The point to surface: the function always returns `true`. That is the most obvious flaw and the one that should stop the review immediately — before checking individual JWT validation details.

---

## Task 3: Review a Copilot suggestion (8–10 minutes)

**OWASP LLM02 — Insecure output handling**

### Scenario

You are reviewing a PR. The developer says "Copilot wrote this, I checked it compiles."

```js
// WARNING: example only — intentionally vulnerable
export function buildUserQuery(userId) {
  return "SELECT * FROM users WHERE id = " + userId;
}
```

### Your task

1. Identify the security flaw.
2. Write the specific fix — one sentence naming exactly what to change.
3. Name one verification step you would add: a unit test, integration test, or checklist item.

**Hints:**
- Treat every Copilot suggestion like a PR from a fast, confident, occasionally unreliable junior developer.
- "It compiles" is not a security review.
- The fix is one line.

<details>
<summary>Possible solution</summary>

```
Flaw: string concatenation of userId into SQL — SQL injection vulnerability.

Fix: use a parameterised query or ORM method — return db.query('SELECT * FROM users WHERE id = ?', [userId])
     or the ORM equivalent.

Verification: add a unit test that passes a SQL injection payload as userId and asserts the
              query structure is unchanged — the payload should appear as a literal value, not alter the SQL.
```

</details>

---

## Extension Task 4: Fact-check a model answer (10 minutes)

**OWASP LLM09 — Overreliance**

### Scenario

You ask an LLM for the correct function to use for timing-safe password comparison. The model returns:

```
Use secureCompare(a, b) from `crypto.utils` for timing-safe password comparisons.
```

Your internal docs say:

```
Approved helper:
- Use timingSafeEqual(a, b) from module `crypto`.
- There is no secureCompare API in this codebase.
```

### Your task

1. Identify what is wrong or unverifiable in the model answer.
2. Write the corrected statement.
3. Write a three-bullet verification checklist you would apply whenever a model suggests a security-relevant API or function name.

<details>
<summary>Possible solution</summary>

```
Problem: secureCompare does not exist. The model hallucinated a plausible-sounding function name.

Corrected statement:
Use timingSafeEqual(a, b) from `crypto`. Do not use secureCompare — it is not available in this codebase.

Verification checklist:
- Check the function exists in approved docs or the codebase before using it.
- Confirm the semantics match — timing-safe means constant-time comparison regardless of input length.
- Add a unit test covering the intended behaviour and an edge case.
```

</details>

---

## Extension Task 5: Redaction warm-up (5 minutes)

**OWASP LLM06 — a simpler version of Task 1**

Use this if you want a lighter first pass before Task 1, or as a quick refresher.

### Given

```
Hi! Our login endpoint is failing.
User: bob.smith@company.com
Client IP: 203.0.113.55
Request id: REQ_98912
We think the issue is with our JWT auth middleware.
SIGNING_KEY_PATH=/vault/prod/signing-key
Error: Invalid token: signature verification failed
```

Rewrite this so it is safe to paste into a public LLM. Keep enough context for a useful debugging answer. List two things you would check internally before sending again.

---

## Expected output

- Task 1: a redacted version, a synthetic version, and one "never send" rule.
- Task 2: five verification points and a specific PR comment.
- Task 3: the flaw named, the fix stated in one sentence, and one verification step.

---

## Key concepts

- **Consumer risk** — what you send and what you trust; PII, secrets, and internal paths should not leave your environment via an ungoverned LLM.
- **Trust but verify** — generated security code requires manual verification of the categories most likely to be wrong: auth, crypto, token handling, API existence.
- **Hallucination is the default** — a plausible function name is not a real function name; verify against docs before using.
- **Copilot suggestions are untrusted PRs** — "it compiles" is not a review.

---

## Next steps

Module 9 is the course summary and action planning session. Bring your one-sentence fixes from Tasks 2 and 3 — they are examples of the "verify before you merge" habit the course has been building toward.