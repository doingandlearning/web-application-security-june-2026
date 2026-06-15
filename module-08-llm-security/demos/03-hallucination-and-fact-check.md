# Demo 3: Hallucinations + Fact-check (LLM09 / LLM02)

## Goal
Show that “plausible” model output can be wrong or unsafe, so you must verify security-sensitive logic against docs/tests.

## Setup (toy example; avoids needing real provider details)
### Prompt
```text
In our stack, what method should we use to compare two passwords in a timing-safe way?
Return a short example.
```

### Example model output (for demo purposes)
```text
Use secureCompare(a, b) from `crypto.utils`.
It is timing-safe and recommended.
```

### “Official docs excerpt” (provided in the repo for teaching)
```text
From our internal docs (excerpt):
- The recommended helper is `timingSafeEqual(a, b)` from `crypto`.
- There is no `secureCompare(a, b)` API in our standard library.
```

## Runbook: how to present it
1. Show the model output.
2. Ask: “What’s wrong with this from a security perspective?”
3. Reveal the excerpt and point out:
   - The API name may not exist.
   - Even if something exists, semantics may differ (not timing-safe).
4. Debrief with “trust but verify”:
   - Verify auth/crypto/tokens, and anything correctness-critical, before accepting.

## Teaching tie-in
- Hallucination is default behaviour; confidence != correctness.
- LLM02: insecure output handling (wrong logic).
- LLM09: overreliance (trusting the draft without verifying).

## Suggested “what to say”
> “Verification is part of engineering quality—LLM output is a draft, not an authoritative source.”

