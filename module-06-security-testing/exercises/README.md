# Module 6 — Exercises

## Lab 6: Security testing and triage

This lab has two tasks. Task 1 is hands-on with OWASP ZAP. Task 2 is a triage decision exercise that can run independently if ZAP is not available or time is short.

Allow 40–50 minutes for both tasks plus the debrief.

---

## Task 1: ZAP baseline scan (20–25 minutes)

### Goal

Run an automated scan against TrustyTickets, read the output, and practise the triage process — reproduce, classify, fix or suppress.

### Prereq

TrustyTickets must be running: `cd TrustyTickets && dotnet run`

If ZAP is not installed, use the pre-captured scan results in `exercises/module-6/zap-sample-output/` and start from Step 2.

---

### Step 1: Run the scan (5 minutes)

Open ZAP. Set the target URL to `http://127.0.0.1:5000` (or the port TrustyTickets is running on).

Run an **automated scan** or **baseline scan**. Wait for it to complete.

Open the Alerts panel. You should see findings grouped by risk level — High, Medium, Low, Informational.

---

### Step 2: Pick one finding (3 minutes)

Choose one **High or Medium** alert. Avoid Informational findings for this exercise — they are hard to reproduce and rarely actionable.

Note:
- The alert name and risk level
- The request ZAP sent
- The response that triggered the flag

---

### Step 3: Reproduce it (5–8 minutes)

Using the browser or browser dev tools, try to reproduce the behaviour ZAP flagged.

Follow the exact request ZAP sent. Confirm whether the response matches what ZAP reported.

**This is the critical step.** A scanner finding is a hypothesis. Reproduction confirms whether it is real.

**Hints:**
- Copy the request from the ZAP alert and replay it in the browser address bar or dev tools network tab.
- If you cannot reproduce it manually, that is useful information — it may be a false positive.
- If the finding relates to a missing header, check the raw response headers in dev tools.

---

### Step 4: Classify and write the fix sentence (3 minutes)

Decide: **true positive** or **false positive**?

- True positive — the vulnerability is real and exploitable in this app's context.
- False positive — ZAP matched a pattern but the finding does not represent real risk here.

Write one sentence:
- If true positive: "To fix this I would…" — name the specific change.
- If false positive: "This is a false positive because…" — name the reason.

---

### Step 5: Chat share

Type in chat: **your finding name, your classification (TP/FP), and your one-sentence fix or reason**.

The facilitator will read a few responses and ask: did anyone classify the same finding differently? If so — what reasoning led to different conclusions?

---

### Hints for common ZAP findings in TrustyTickets

| Finding | Likely classification | Note |
|---|---|---|
| SQL injection | True positive | Matches the Module 3 vulnerability |
| XSS | True positive | Matches the Module 3 vulnerability |
| Missing security headers | True positive | Straightforward to fix |
| Auto-complete enabled on password field | Low / informational | Debatable — browser-dependent |
| Server leaks version information | Medium | True positive but low immediate risk |

---

<details>
<summary>Triage process reminder</summary>

```
1. Reproduce — follow ZAP's request and confirm the behaviour manually
2. Decide — true positive or false positive?
3. Act:
   - True positive → fix or accept with written justification
   - False positive → document and suppress in ZAP
4. Baseline — configure ZAP to fail builds only on new critical/high findings, not on known-and-accepted ones
```

</details>

---

## Task 2: Triage eight findings (15–20 minutes)

This exercise can run alongside Task 1 or independently. It uses a fixed set of findings so every delegate is working from the same material.

---

### Your task

For each finding below, decide:

1. **Classification** — true positive / likely false positive / needs more context
2. **Severity** — Critical / High / Medium / Low
3. **Action** — fix now / add to backlog / accept risk

Be ready to explain your reasoning. There are no single correct answers — the goal is defensible reasoning, not a lookup.

---

### Finding 1 — SQL query construction

```js
const query = "SELECT * FROM users WHERE id = " + userInput
```

User input comes from a query parameter.

---

### Finding 2 — Hardcoded API key

```python
API_KEY = "sk_live_123456789"
```

Found in backend code used for payments.

---

### Finding 3 — Outdated dependency

```
lodash 4.17.15 (known vulnerability)
```

Used in a development script, not production code.

---

### Finding 4 — Session cookie configuration

```http
Set-Cookie: sessionId=abc123
```

No additional flags are set.

---

### Finding 5 — Public storage bucket

```hcl
resource "aws_s3_bucket" "assets" {
  acl = "public-read"
}
```

Bucket is used to serve images on a public website.

---

### Finding 6 — Reflected input in template

```html
<p>Hello {{ userInput }}</p>
```

Application uses a framework that escapes output by default.

---

### Finding 7 — Error message returned to client

```json
"error": "SQLSTATE[42P01]: Undefined table: users"
```

This message is returned in API responses.

---

### Finding 8 — Password policy

```
Minimum password length: 6 characters
```

No additional complexity requirements.

---

### Discussion prompts

- What assumptions are you making?
- What would make this more or less dangerous?
- Does context change your decision?
- What would an attacker need to exploit this?

---

### Step: Chat share

Type in chat: **the finding number you found hardest to classify and why**.

The facilitator will discuss the edge cases — Finding 3 (dev-only dependency), Finding 5 (intentionally public bucket), and Finding 6 (framework auto-escaping) are the ones most likely to produce disagreement.

---

## Expected output

- Task 1: a finding name, a TP/FP classification, and a one-sentence fix or suppression reason.
- Task 2: a completed classification for all eight findings, with reasoning for the three edge cases.

---

## Key concepts

- **Scanners find candidates** — reproduction confirms whether a finding is real.
- **Triage discipline** — reproduce, decide, act, baseline; without this, alerts become noise.
- **Context changes severity** — a public bucket serving public assets is not the same finding as a public bucket containing customer data.
- **False positives need documentation** — suppress with a written reason, not a silent ignore.

---

## Next steps

Module 7 covers wiring these tools into the pipeline — SAST, DAST, and SCA running automatically on every change, with a policy that makes the output actionable rather than overwhelming.