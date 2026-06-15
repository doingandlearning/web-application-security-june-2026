# Demo 4: Copilot Safe vs Unsafe (LLM02 / LLM09)

## Goal
Reinforce “treat suggestions like untrusted PRs” and show a simple verify-before-merge checklist.

## Setup
### “Unsafe” Copilot-style suggestion (example snippet)
```js
// BAD: example only
function validateJwt(token) {
  const payload = decodeBase64(token); // assume it returns JSON
  return true; // no signature check, no exp check
}
```

### “What you’d verify” checklist (talking points)
- Does it validate signatures correctly (no `alg: none` / incorrect verification)?
- Does it check expiry (`exp`) and issuer/audience if required?
- Does it use our approved crypto/tokens libraries?
- Are edge cases handled (malformed tokens, missing fields)?
- Would this logic pass our existing unit tests (or can we add them)?

## Runbook: how to present it
1. Show the unsafe suggestion.
2. Ask learners to identify what is missing or risky (auth/crypto/verification).
3. Then show the rule:
   - “Never accept code you don't understand.”
   - “Verify security-sensitive logic against docs and tests.”

## Teaching tie-in
- LLM02: insecure output handling (accepting wrong logic).
- LLM09: overreliance (trusting the draft without review).

## Suggested “what to say”
> “Copilot is a fast draft generator. The security bar is what you verify and test before merge.”

