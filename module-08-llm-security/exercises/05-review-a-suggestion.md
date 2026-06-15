# Lab 5: Review a suggestion (LLM02 / safer Copilot use)

## Objective
Practice finding one concrete security flaw in “model/Copilot-style” code and proposing a safe correction or verification step.

## Scenario
An LLM suggests a short snippet. It compiles in theory, but it contains a security flaw.

## Given (suggested code; intentionally vulnerable)
```js
// BAD: example only
export function buildUserQuery(userId) {
  return "SELECT * FROM users WHERE id = " + userId;
}
```

## Task
1. Identify the security flaw in this snippet.
2. Describe how you would fix it (parameterisation / validation / using approved data access layer).
3. Provide one verification step you would add (unit test, integration test, or review checklist).

## Hints
- Treat the suggestion like an untrusted PR.
- If it touches queries/auth/crypto, verify with tests and approved libraries.

<details>
<summary>Possible direction (not the only correct solution)</summary>

```text
Fix direction:
- Use parameterised queries or an ORM query builder.
- Validate/parse userId to the expected type.
Verification:
- Add a unit test proving injection payloads do not alter the query structure.
```

</details>

