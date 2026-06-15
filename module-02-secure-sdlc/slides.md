---
title: "**Introduction to Application Security**"
sub_title: Module 2 — Secure SDLC
author: Kevin Cunningham
---

## A finding lands on a Friday afternoon

Your pen test report arrives. Twelve findings. Three rated critical.

One of them is SQL injection in a feature you shipped eight months ago.

<!-- pause -->

**Type in chat: whose job is it to fix this — dev / security team / both / it depends**

We'll come back to this at the end of the module.

<!-- speaker_note: Hold back. Most will say "both" or "it depends" — push them to commit to one. The tension between dev ownership and security team accountability is the module's central concept. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Security and the delivery lifecycle
===

<!-- end_slide -->

## Where security usually lives

<!-- incremental_lists: true -->

- A single review gate before go-live
- An annual pen test followed by a long findings backlog
- Security team as external gatekeeper — dev teams wait for sign-off

<!-- incremental_lists: false -->

<!-- pause -->

The problem is not that security exists. It is that it arrives too late to be cheap.

<!-- speaker_note: Ask "hands up if any of those sound familiar" — almost every team recognises the annual pen test model. Don't dwell; the cost framing on the next slide makes the point. -->

<!-- end_slide -->

## The cost of finding things late

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

A vulnerability found at **design** costs a conversation.

Found at **code review** — a PR comment and a fix.

Found at **QA** — a bug ticket and a sprint item.

Found at **production** — an incident, a patch release, customer impact, and a pen test finding.

<!-- column: 1 -->

<!-- new_lines: 3 -->

```
Design      ← cheapest
    ↓
Code review
    ↓
QA / staging
    ↓
Production  ← most expensive
```

<!-- reset_layout -->

<!-- pause -->

**Shift left** means finding things earlier — not adding more gates.

<!-- end_slide -->

## Security in each phase

<!-- column_layout: [1, 2] -->

<!-- column: 0 -->

**Requirements**
What needs protecting? Data classifications, compliance constraints.

**Design**
Data flows, trust boundaries, threat sketches.

**Implementation**
Secure coding patterns, library choices.

**Review and test**
Checklists, SAST, dependency scans.

**Deploy and operate**
Secrets management, monitoring, incident response.

<!-- column: 1 -->

<!-- new_lines: 1 -->

**Type in chat — which phase does your team most often skip?**

<!-- pause -->

The goal is not to do everything in every phase. It is to make security a normal part of each one — not a separate lane.

<!-- reset_layout -->

<!-- speaker_note: Common answers are design and requirements — most teams go straight to implementation. That is what the threat modelling section addresses. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Threat modelling
===

<!-- end_slide -->

## The simplest threat model

You do not need a big document. You need a sketch and three questions.

<!-- pause -->

```
[Browser] → [API] → [Database]
                 ↘ [External service]
```

<!-- pause -->

1. Where is input untrusted?
2. Where is sensitive data stored or processed?
3. What could go wrong at each boundary?

<!-- speaker_note: Draw this on a whiteboard or share screen with the diagram. The point is that threat modelling is a conversation, not a compliance artefact. -->

<!-- end_slide -->

## Threats and risk

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Threat**
A specific "what could go wrong" — SQL injection, IDOR, data leak, session hijack.

**Risk**
Likelihood × impact. Or simply High / Medium / Low.

<!-- column: 1 -->

**Why it matters**

Without a risk rating, everything is P1 and nothing gets prioritised.

With one, you can decide what to fix now, what to mitigate, and what to accept.

<!-- reset_layout -->

<!-- end_slide -->

## STRIDE — a thinking checklist

Use this when "what could go wrong?" stalls out.

<!-- pause -->

| Letter | Threat | Example |
|---|---|---|
| S | Spoofing | Login bypass, forged identity |
| T | Tampering | Modified request, altered DB record |
| R | Repudiation | Action taken with no audit trail |
| I | Information disclosure | Data returned to wrong user |
| D | Denial of service | Endpoint overwhelmed, feature unavailable |
| E | Elevation of privilege | User → admin without authorisation |

<!-- pause -->

Use it as a prompt — not a checklist to complete in full every time.

<!-- speaker_note: Walk through one or two rows with the TrustyTickets system. "Where could spoofing happen in this app?" gets more traction than defining the acronym first. -->

<!-- end_slide -->

## Exercise: Threat sketch

Take the TrustyTickets system from Module 1 (or your own app).

**Individually (3 minutes):**

List three threats — one sentence each. Use STRIDE as a prompt if you need it.

Then decide: which one would you fix first, and why?

<!-- pause -->

**In pairs:** compare your top-priority threat. Did you pick the same one? If not — why not?

<!-- speaker_note: The disagreement between pairs is the point. Risk prioritisation is a judgement call, not a lookup. Push pairs to surface what assumptions led to different rankings — likelihood, impact, exploitability. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Security automation in CI/CD
===

<!-- end_slide -->

## The pipeline as a security checkpoint

```
commit → PR → build → test → staging → production
```

<!-- pause -->

Four places to add automated security checks — each with a different trade-off.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Dependency scan**
Checks libraries for known CVEs.
Runs on: PR or build.

**SAST**
Static analysis — injection patterns, unsafe APIs, hardcoded secrets.
Runs on: PR or build.

<!-- column: 1 -->

**Secret detection**
Leaked keys and passwords in the repo.
Runs on: commit or PR.

**DAST**
Dynamic scan of a running app.
Runs on: staging (covered in Module 6).

<!-- reset_layout -->

<!-- end_slide -->

## Tuning and gates — the real decision

Adding a tool is easy. Deciding what to do with its output is not.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Start with warnings**
Let findings accumulate while you understand the noise level.

**Agree a gate threshold**
Critical only? High and above? Who decides?

**Document risk acceptance**
Who can sign off a known finding, and for how long?

<!-- column: 1 -->

**The failure mode to avoid**

A gate that fires on everything becomes a gate everyone learns to bypass.

A gate nobody trusts is worse than no gate.

<!-- reset_layout -->

<!-- speaker_note: Ask "has anyone worked in a team where the security gate was routinely overridden?" — this is extremely common. The answer is usually that the threshold was set too aggressively before the team trusted the tool. -->

<!-- end_slide -->

## Exercise: Where does your gate go?

**In breakout rooms (5 minutes):**

Your team ships a Node.js API. You can add one automated security check to the pipeline this sprint.

<!-- pause -->

Choose one and defend it:

- **Dependency scan on every PR** — blocks merge if a critical CVE is found
- **Secret detection on commit** — blocks push if a key pattern is detected
- **SAST on build** — warns on high-severity findings, does not block

<!-- pause -->

What did you choose, and what would you tell a sceptical senior dev who thinks it will slow the team down?

<!-- speaker_note: Pre-assign rooms. Listen for teams that pick SAST — they often underestimate the noise on a first run. The secret detection option tends to get consensus fastest because the false-positive rate is low and the impact of a leak is obvious. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Ownership and the security team
===

<!-- end_slide -->

## Who owns security outcomes?

Three models teams use — each with different consequences.

<!-- pause -->

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**Security team owns it**
Dev ships. Security reviews. Findings go back to dev.

Consequence: bottleneck, late findings, adversarial relationship.

<!-- pause -->

<!-- column: 1 -->

**Dev team owns it**
Dev ships with security practices baked in. Security team sets policy and tooling.

Consequence: faster fixes, earlier findings, requires investment in training.
<!-- pause -->
<!-- column: 2 -->

**Shared ownership**
Dev owns implementation. Security owns policy and escalation. Product owns risk decisions.

Consequence: needs clear boundaries — otherwise everything defaults to "it depends".

<!-- reset_layout -->

<!-- pause -->

**Type in chat — which model does your team actually run?**

<!-- speaker_note: Most teams will say "shared" but describe something closer to the first model in practice. The distinction between "policy owner" and "implementation owner" is worth unpacking if there is time. -->

<!-- end_slide -->

## Back to Friday afternoon

The pen test found SQL injection in a feature you shipped eight months ago.

You said: **dev / security team / both / it depends**.

<!-- pause -->

The fix is a dev task — parameterised query, one line.

But why did it get to production? That is a process question.

<!-- pause -->

If security only shows up as a pen test finding, the process has a gap — not just the code.

**Shift left means closing that gap before production, not after.**

<!-- speaker_note: Return to the chat poll answers from the opening. If most said "both" — agree, but push on what "both" means in practice. The finding is dev's to fix. The process that let it through is shared. -->

<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **Shift left** — security found at design costs a conversation; found in production costs an incident
2. **Threat modelling** — a sketch and three questions is enough to start; STRIDE gives you prompts when you stall
3. **Automation** — dependency scans, SAST, and secret detection on every change; tune gates before you enforce them
4. **Ownership** — dev teams fix vulnerabilities; they need process and tooling that makes that possible earlier

<!-- end_slide -->

## Bridge to Module 3

**We've established:**

<!-- incremental_lists: true -->

- Where in the lifecycle security belongs
- How to sketch threats and prioritise by risk
- What automated checks go where in the pipeline

<!-- incremental_lists: false -->

**Module 3 — Common vulnerabilities:** you'll exploit real weaknesses in DVWA and WebGoat before fixing and automating them in later modules.

Knowing what "broken" looks like makes every earlier decision concrete.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===

*Introduction to Application Security — Module 2*