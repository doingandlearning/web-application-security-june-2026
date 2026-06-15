# Triage Security Findings

You’ve been given a set of security findings from a recent scan.

Your job is to **triage each finding**.

---

## Your Task

For each finding, decide:

1. **Classification**

   * True positive
   * Likely false positive
   * Needs more context

2. **Severity**

   * Low
   * Medium
   * High
   * Critical

3. **Action**

   * Fix now
   * Add to backlog
   * Accept risk

---

## Findings

---

### 1. SQL Query Construction

```js
const query = "SELECT * FROM users WHERE id = " + userInput
```

User input comes from a query parameter.

---

### 2. Hardcoded API Key

```python
API_KEY = "sk_live_123456789"
```

Found in backend code used for payments.

---

### 3. Outdated Dependency

```text
lodash 4.17.15 (known vulnerability)
```

Used in a development script, not production code.

---

### 4. Session Cookie Configuration

```http
Set-Cookie: sessionId=abc123
```

No additional flags are set.

---

### 5. Public Storage Bucket

```hcl
resource "aws_s3_bucket" "assets" {
  acl = "public-read"
}
```

Bucket is used to serve images on a public website.

---

### 6. Reflected Input in Template

```html
<p>Hello {{ userInput }}</p>
```

Application uses a framework that escapes output by default.

---

### 7. Error Message Returned to Client

```json
"error": "SQLSTATE[42P01]: Undefined table: users"
```

This message is returned in API responses.

---

### 8. Password Policy

```text
Minimum password length: 6 characters
```

No additional complexity requirements.

---

## Discussion Prompts (if you need them)

* What assumptions are you making?
* What would make this more or less dangerous?
* Does context change your decision?
* What would an attacker need to exploit this?

---

## Outcome

Be ready to explain:

* Why you classified it that way
* Why you chose that severity
* Why you chose that action

There are no perfect answers — the goal is **reasoning, not guessing**.
