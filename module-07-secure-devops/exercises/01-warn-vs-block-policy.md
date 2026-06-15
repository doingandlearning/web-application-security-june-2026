# Lab 7.1 — Warn vs block policy

## Objective
Decide what should block a pipeline run vs what should warn and create a ticket. Your goal is to turn security scanning into **fast, reliable decisions** instead of noisy alert spam.

You will:
1. Classify sample findings by severity and risk clarity
2. Choose an action: **block**, **warn+ticket**, or **report only**
3. Write a short “policy rule” you could document in your repo

---

## Scenario: “We have scanner output, but no policy”

Your team runs a few security tools:
- secret scanning
- SCA (dependency CVEs)
- SAST (code patterns)
- optional DAST / IaC checks

You now have real findings, but:
- the team disagrees about what to do first
- some findings are clearly dangerous, others are ambiguous
- ignoring everything leads to burnout

---

## Task 1: Decide actions for findings

For each finding below, fill in:
- **Action**: `BLOCK` / `WARN + TICKET` / `REPORT ONLY`
- **Why**: 1 sentence using severity + “risk clarity”
- **Ownership**: who should fix/triage first (`Dev`, `Dev+Sec`, or `Sec`)

### Finding set

#### 1) Leaked secret

```text
Found: `API_KEY=sk_live_9f8e...` in commit message / config file
Source: secret scanning
```

#### 2) Critical runtime dependency vuln (fix available)

```text
Dependency: `openssl` in runtime dependency graph
Severity: CRITICAL
Fix available: YES
Exploitability: likely
```

#### 3) High SAST finding, uncertain reachability

```text
Pattern: potential injection sink
Severity: HIGH
Reachability: unclear (may be dead code / not reachable in common flows)
```

#### 4) Medium SCA vuln (no direct fix yet)

```text
Dependency: `libcurl` transitive
Severity: MEDIUM
Fix available: NO (or upgrade requires a bigger change)
```

#### 5) Security header missing (browser-facing issue)

```text
Alert: missing `X-Frame-Options` / weak security headers
Severity: MEDIUM
Context: reduces protection, doesn’t usually expose data directly
```

#### 6) IaC misconfiguration (potential exposure)

```text
Alert: storage bucket or DB appears public
Severity: HIGH
Context: might be only for static assets, might be a real exposure
```

---

## Task 2: Write a 3-line policy rule

Write your “repo friendly” rule in this format:

1. `BLOCK` when: ________
2. `WARN + TICKET` when: ________
3. `REPORT ONLY` when: ________

Include one line about:
- how you handle false positives / uncertainty
- whether you baseline older findings (fail builds only on new high-risk findings)

---

## Hints (use if you get stuck)

- “Block when the risk is clear and the action is reversible.”
- “Warn when context is needed and give a clear SLA/ticket path.”
- “Tools find candidates; you decide what matters.”

---

## Example output

You should end with:
- a filled decision table for all 6 findings
- a short policy snippet (3 lines) you could paste into a README

---

## Key concepts demonstrated

- Security tooling becomes useful only with **policy**
- Warn vs block is about **risk clarity**, not just severity labels
- Reducing noise means **baseline + dedupe + triage discipline**

