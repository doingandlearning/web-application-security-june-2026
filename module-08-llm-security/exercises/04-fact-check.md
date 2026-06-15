# Lab 4: Fact-check a model answer (LLM09)

## Objective
Practice verifying a model’s “fact” against known-good documentation.

## Scenario
You ask an LLM for the correct API/method name. The model returns something plausible. Your task is to verify and correct it.

## Task
1. Identify what looks wrong or unverifiable in the “model answer”.
2. Use the provided “docs excerpt” to produce a corrected statement.
3. Write a short “verification checklist” (3 bullets) you would apply next time.

## Model answer (possibly wrong)
```text
Use secureCompare(a, b) from `crypto.utils` for timing-safe password comparisons.
```

## Docs excerpt (truth source for the lab)
```text
Approved helper in our codebase:
- Use timingSafeEqual(a, b) from module `crypto`.
- There is no secureCompare API in our standard library.
```

## Hints
- Confidence != correctness.
- For security-sensitive code, verify against official docs and/or your internal approved libraries.

<details>
<summary>Example corrected output (format guidance)</summary>

```text
Corrected statement:
Use timingSafeEqual(a, b) from `crypto`.
Do not use secureCompare because it is not available in our standard library.
Verification checklist:
- Check API existence and signature in approved docs.
- Confirm semantics (timing-safe) and edge cases.
- Add/adjust unit tests to cover the intended behaviour.
```

</details>

