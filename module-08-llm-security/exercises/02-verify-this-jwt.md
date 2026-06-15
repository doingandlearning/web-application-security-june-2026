# Lab 2: Verify “JWT validation” (LLM02 + LLM09)

## Objective
Practice “trust but verify” by spotting what must be verified when you accept LLM-generated security logic.

## Scenario
An LLM suggests a JWT validation function. It looks plausible. Your job is to identify what is risky or missing.

## Task
1. List at least 5 verification points you would check before merging/deploying the suggested code.
2. For each verification point, state:
   - what could be wrong
   - what you would check (docs, unit tests, config, existing auth library patterns)

## Given (model-suggested, intentionally flawed)
```js
// BAD: example only (intentionally missing checks)
function validateJwt(token) {
  const payload = JSON.parse(atob(token.split('.')[1]));
  // If header.alg is missing/none, still accept.
  // If exp is missing, still accept.
  return true;
}
```

## Hints
- Separate “looks correct” from “is correct”.
- Anything touching auth/crypto/tokens must be verified (LLM02 + LLM09).

<details>
<summary>Example answer shape (use as guidance)</summary>

```text
- Signature verification: check key/algorithm and ensure token is verified cryptographically.
- exp/nbf checks: ensure expiry and not-before are enforced.
- iss/aud checks (if your system uses them): validate expected issuer/audience.
- Error handling: malformed tokens must fail closed (no “return true”).
- Unit tests: add tests for expired token, wrong signature, wrong audience, malformed structure.
```

</details>

