# Module 7 — Exercises

## Lab 7: Secure DevOps

This lab has two required tasks and two optional extensions. Tasks 1 and 2 are the priority — both produce something a team can use immediately. Task 3 (container checklist) is for delegates who work with containers or finish early. Task 4 (WSTG pipeline mapping) connects the OWASP Testing Guide to your SDLC phases and scanner output — use it if you want to tie Modules 2, 6, and 7 together.

Allow 40–50 minutes for Tasks 1 and 2 plus debriefs. Add 20–25 minutes for Task 4 if time allows.

---

## Task 1: Warn vs block policy (15–20 minutes)

### Scenario

Your team runs secret scanning, SCA, SAST, and optional DAST and IaC checks. You now have real findings but no agreed policy — the team disagrees about what to do first, some findings are clearly dangerous and others are ambiguous, and ignoring everything leads to alert fatigue.

---

### Step 1: Classify six findings (10 minutes)

For each finding below, decide:
- **Action** — `BLOCK` / `WARN + TICKET` / `REPORT ONLY`
- **Why** — one sentence using severity and risk clarity
- **Ownership** — who should fix or triage first: `Dev`, `Dev+Sec`, or `Sec`

---

**Finding 1 — Leaked secret**

```
Found: API_KEY=sk_live_9f8e... in commit message / config file
Source: secret scanning
```

**Finding 2 — Critical runtime dependency CVE (fix available)**

```
Dependency: openssl in runtime dependency graph
Severity: CRITICAL
Fix available: YES
Exploitability: likely
```

**Finding 3 — High SAST finding, uncertain reachability**

```
Pattern: potential injection sink
Severity: HIGH
Reachability: unclear (may be dead code / not reachable in common flows)
```

**Finding 4 — Medium SCA vuln, no fix yet**

```
Dependency: libcurl (transitive)
Severity: MEDIUM
Fix available: NO (upgrade requires a larger change)
```

**Finding 5 — Missing security headers**

```
Alert: missing X-Frame-Options / weak security headers
Severity: MEDIUM
Context: reduces protection, does not usually expose data directly
```

**Finding 6 — IaC misconfiguration**

```
Alert: storage bucket or database appears public
Severity: HIGH
Context: may be intentional for static assets, or may be a real exposure
```

---

### Step 2: Write a three-line policy rule

Write your policy in this format, as if it were going into your repo README:

1. `BLOCK` when: ________
2. `WARN + TICKET` when: ________
3. `REPORT ONLY` when: ________

Add one line covering:
- how you handle false positives and uncertainty
- whether you baseline older findings (fail builds only on new high-risk findings)

**Hints:**
- "Block when the risk is clear and the action is reversible."
- "Warn when context is needed — give a clear SLA and a ticket path."
- Findings 3 and 6 are the interesting edge cases. Both are technically HIGH but the right action depends on context. If you classify them the same way, ask yourself whether the reasoning is actually the same.

<details>
<summary>Example policy</summary>

```
BLOCK when: confirmed secret detected anywhere; critical CVE in runtime dependency with fix available;
            HIGH SAST finding with confirmed reachability.
WARN + TICKET when: HIGH finding with unclear reachability (SLA: triage within 5 days);
                    MEDIUM finding with no fix available (SLA: review in next sprint);
                    IaC misconfiguration requiring context check before action.
REPORT ONLY when: informational / low-confidence findings; dev-only dependency CVEs.

False positives: suppress with written justification; review suppressions quarterly.
Baseline: fail builds on new HIGH+ findings only; existing findings tracked as debt.
```

</details>

---

### Debrief (chat share)

Type in chat: **Finding 3 or Finding 6 — BLOCK, WARN, or REPORT? One sentence explaining why.**

The facilitator will read the responses. If the room splits — that is the point. Block vs warn on a HIGH finding with uncertain context is a judgement call, not a lookup. The goal is a written policy that makes the call explicit so it does not have to be relitigated on every PR.

---

## Task 2: Minimum runbook — suspected credential leak (20–25 minutes)

### Scenario

Your pipeline flags a possible credential leak — a secret scanner detected something that looks like an API key in a commit or build log. You also see suspicious 401/403 patterns after the commit was deployed.

You do not have full confirmation yet, but you act as if this is real until proved otherwise.

---

### Step 1: Fill the runbook structure (15 minutes)

Write short bullet steps for each section. This is a working document, not an essay.

**A) Detect and confirm**
- What triggers this runbook?
- What evidence do you check in the first 10–20 minutes?
- What is your definition of "suspected" versus "confirmed"?

**B) Triage**
- Which systems and environments might be affected?
- What data or actions might be exposed?
- Who needs to be involved — Dev, Sec, Ops?

**C) Contain**
Write 2–4 containment actions. For each, add: why this first, and who executes it.

Examples to draw from:
- Revoke or rotate the affected credential
- Invalidate active sessions or tokens that used it
- Disable the affected feature or endpoint behind a flag
- Block or throttle suspicious traffic

**D) Eradicate and recover**
- What is the root-cause fix?
- What rotation and redeploy steps follow the patch?
- How do you verify recovery?

**E) Learn**
- What pipeline check should be tightened — secret scan policy, baseline, SCA?
- What monitoring trigger should be added or adjusted?
- What updates to this runbook did you identify?

---

### Step 2: Add three escalation triggers

Write three escalation triggers in this format:

- Escalate when: ________
- Escalate when: ________
- Escalate when: ________

Make them specific enough to be actionable — "things get bad" is not an escalation trigger.

---

### Debrief (chat share)

Type in chat: **your fastest first action in the credential-leak scenario — one sentence.**

The facilitator will read the responses. The question to surface: did anyone put "investigate" before "rotate"? Rotating the credential is the correct first action — it stops the bleeding. Investigating whether it was used comes immediately after, not before. Teams that investigate first are still exposed while they investigate.

---

## Extension Task 3: Container security checklist (optional, 15 minutes)

Use this if you work with containers or finish Tasks 1 and 2 early.

### Scenario

Your team is containerising TrustyTickets. A scan flagged known vulnerabilities in the base image, the container appears to run as root, and secrets may be baked into image layers.

---

### Step 1: Review the Dockerfile and find issues

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:latest

# secret baked into image layers
COPY appsettings.json /app/appsettings.json

WORKDIR /app
COPY . .

# no USER directive — runs as root
ENTRYPOINT ["dotnet", "TrustyTickets.dll"]
```

Write down:
- Two build-time issues
- One issue related to secrets handling
- One issue related to runtime containment

---

### Step 2: Convert to three asks

Produce exactly three asks for the delivery team — one for base image management, one for secrets handling, one for runtime containment:

- `Ask 1:` ________
- `Ask 2:` ________
- `Ask 3:` ________

---

### Step 3: Map to pipeline placement

Where in the pipeline would you enforce each ask?

- `Build / package:` ________
- `Deploy / run:` ________

<details>
<summary>Possible solution</summary>

```
Issues found:
- latest tag on base image — no pinned version, vulnerabilities in the image are unknown
- appsettings.json copied into the image — secrets baked into layers
- no USER directive — process runs as root inside the container

Asks:
Ask 1: Pin the base image to a specific digest; scan it during the build and fail on critical findings.
Ask 2: Remove secrets from appsettings.json; inject them at runtime via environment variables or a vault.
Ask 3: Add a USER directive to run as a non-root user; set a read-only root filesystem where possible.

Pipeline placement:
Build / package: image scan, USER directive check, secret detection in layers
Deploy / run:    runtime policy enforcement (non-root, read-only filesystem, network egress limits)
```

</details>

---

## Extension Task 4: Map OWASP WSTG to your pipeline (optional, 20–25 minutes)

The WSTG is a methodology and checklist library — not a tool you run. Use it as the test-case backlog behind SAST, DAST, and manual review.

Full instructions: [04-wstg-pipeline-mapping.md](04-wstg-pipeline-mapping.md)

**Task 1 — Phase mapping (10 minutes)**

Map each of the five WSTG testing phases to one concrete TrustyTickets activity and one tool or artefact (standards, threat model, SAST, ZAP, regression tests).

**Task 2 — Cheapest phase to catch it (8 minutes)**

For SQLi, IDOR, stored XSS, and missing security headers: which WSTG phase catches each cheapest, with what control, and why?

**Task 3 — WSTG rubric for ZAP alerts (7 minutes)**

Classify three sample ZAP alerts by WSTG area, triage true/false positive, choose a pipeline action, and name the earlier phase that should have caught each issue.

**Debrief:** Alert A — which WSTG phase should have caught this first, and with what control? One sentence per pair.

---

## Expected output

- Task 1: a completed decision table for all six findings plus a three-line policy snippet.
- Task 2: a completed five-section runbook plus three escalation triggers.
- Extension Task 3: three asks and their pipeline placement.
- Extension Task 4: WSTG phase mapping table, four-row vulnerability table, and triage for three ZAP alerts.

---

## Key concepts

- **Policy makes tooling useful** — without a written warn/block policy, alerts become noise and teams route around them.
- **Contain before you investigate** — rotating the credential stops the bleeding; investigating whether it was used comes immediately after.
- **Blast radius reduction** — container hardening, least-privilege IAM, and short-lived credentials limit what an attacker can reach if something is compromised.

---

## Next steps

Module 8 applies the same DevSecOps thinking to LLM features and AI-assisted development — prompt injection, data leakage, and governance for AI tooling in your workflow.