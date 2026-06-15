# Demo 7.1 — Secure pipeline gate (warn vs block)

## Objective
Show what it looks like to embed security checks into CI/CD and how to decide whether they:
- **block** a merge/release
- **warn** and create a ticket
- are just **reported** (no build impact)

This demo is intentionally tool-agnostic: you can substitute the exact SCA/SAST/secret scanner your team uses.

---

## What to show (live)

1. A minimal pipeline outline (build → security checks → test → package/deploy)
2. One “security job” with three categories of checks:
   - **Secrets scan** (commit/PR level)
   - **SCA / dependency scan** (build/PR level)
   - **SAST** (changed code path / PR level)
3. A simple policy decision:
   - leaked secret → fail/block
   - critical/high dependency vuln → fail/block (unless approved)
   - high SAST → fail/block
   - medium findings → warn + ticket + SLA

---

## Example snippet (GitHub Actions-style, conceptual)

```yaml
security:
  runs-on: ubuntu-latest
  steps:
    - uses: actions/checkout@v4

    # 1) Secrets scanning (block immediately)
    - name: Secret scan (gitleaks or equivalent)
      run: ./tools/secret-scan --fail-on=high

    # 2) Dependency scanning (SCA)
    - name: Dependency scan (SCA)
      run: ./tools/dependency-scan --fail-on=critical,high

    # 3) Static analysis (SAST)
    - name: SAST (changed code)
      run: ./tools/sast --fail-on=high

    # Output findings so the team can triage
    - name: Publish findings summary
      run: ./tools/security-summary --format=markdown
```

## Talk track (say this)

- “The goal is not maximum scanning. The goal is fast, reliable security decisions.”
- “Tools are helpers. Policy is what makes them useful.”
- “If the action is reversible (and risk is uncertain), we warn. If the risk is clear, we block.”

---

## Ask the room (30 seconds each, in chat or verbally)

1. “If a secret is detected in a commit, is that warn or block?”
2. “If SCA says there’s a critical CVE in a runtime dependency, should we block by default?”
3. “If SAST finds something high severity but we’re not sure it’s reachable, do we block or warn?”

---

## Suggested “demo outcome”

Show a pretend pipeline result with:
- one step marked **failed** due to secrets / critical dependency
- one step marked **passed** but with **warnings** (medium SAST)

Then tie it back to the slide:
“Block when the risk is clear and reversible. Warn when context is needed.”

---

## Instructor notes (optional)
- If learners ask “what about false positives?”, answer:
  - baseline existing findings
  - fail only on **new** high-risk findings
  - require justification for suppressions
