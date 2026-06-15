---
title: "**Introduction to Application Security**"
sub_title: Module 7 — Secure DevOps
author: Kevin Cunningham
---

## A credential in the pipeline

A developer adds a database connection string to CI environment variables so the integration tests can run.

Six months later the repo is made public by mistake. The CI variables are not in the repo — but the logs are.

<!-- pause -->

**Type in chat — what should the team do in the next 60 minutes?**

We'll come back to this at the end of the module.

<!-- speaker_note: Listen for answers that include rotating the credential, checking access logs for the DB, notifying relevant parties. Most developers know to rotate the credential but fewer think to check the logs first to understand if it was used. The module builds toward a structured incident response process. -->

<!-- end_slide -->

<!-- jump_to_middle -->

The secure DevOps model
===

<!-- end_slide -->

## Four goals, not one

Security and delivery speed are not opposites — if security is embedded into the workflow rather than added on top.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Prevent**
Stop bad changes from shipping — secrets scanning, SCA, SAST at commit and PR.

**Reduce blast radius**
Assume something gets through — least privilege, container hardening, short-lived credentials.

<!-- column: 1 -->

**Detect**
Find problems quickly once they are in production — monitoring, alerting, anomaly signals.

**Respond**
Act consistently when something goes wrong — runbooks, clear ownership, practised process.

<!-- reset_layout -->

<!-- pause -->

The goal is fast, reliable security decisions — not maximum scanning.

<!-- speaker_note: Ask "which of these four does your team currently do well?" Most teams will say prevent (or nothing). Blast radius reduction and detection are the most commonly absent. -->

<!-- end_slide -->

## Security through the pipeline

Six places to add security — each with different trade-offs on speed and coverage.

<!-- pause -->

| Stage | What fits here |
|---|---|
| Commit / PR | SAST, secrets scanning, policy checks |
| Build | Dependency scan (SCA), SBOM generation |
| Test | DAST, API security tests, IaC checks |
| Package | Container image scanning, signing |
| Deploy | Admission checks, config validation |
| Run | Runtime monitoring, alerting, incident response |

<!-- pause -->

Checks are most effective close to where risk is introduced. You do not need all of these at once.

<!-- end_slide -->

## Minimum viable secure pipeline

If you add only four things this quarter, add these.

<!-- pause -->

<!-- incremental_lists: true -->

- **Secrets scanning** on commits and PRs — block immediately on a confirmed secret
- **SCA** on every PR and build — dependency CVEs caught before they ship
- **SAST** on changed code paths — or at minimum a nightly full scan
- **A written warn/block policy** — what blocks the build, what warns, who can override and how

<!-- incremental_lists: false -->

<!-- pause -->

The policy is not optional. Without it, the tools produce noise and the team routes around them.

<!-- speaker_note: Connect back to Module 2 — this is the pipeline exercise from SDLC, now made concrete. Ask "if you were doing this tomorrow, which would you start with?" — almost always SCA because it is the lowest friction and highest signal. -->

<!-- end_slide -->

## Warn vs block — the policy decision

> "Block when the risk is clear and the action is reversible. Warn when context is needed."

<!-- pause -->

| Finding | Decision |
|---|---|
| Leaked secret | Block immediately |
| Critical CVE in a runtime dependency | Block (unless an approved exception exists) |
| High SAST finding with a confirmed exploit path | Block |
| Medium finding with uncertain context | Warn — create a ticket with an SLA |
| Informational or low-confidence | Report only |

<!-- pause -->

Policy must be explicit, documented, and visible to the whole team. An undocumented gate is a gate everyone learns to bypass.

<!-- end_slide -->

## Reducing noise so decisions happen faster

Alert volume is not a security metric. Decision speed is.

<!-- pause -->

<!-- incremental_lists: true -->

- Baseline existing findings and fail builds only on **new** high-risk issues — not on accumulated debt
- Deduplicate findings across tools — the same root cause can appear as multiple alerts
- Track and suppress false positives with written justification
- Measure time to triage and time to remediate — not number of scans run

<!-- incremental_lists: false -->

<!-- end_slide -->

<!-- jump_to_middle -->

Container and cloud security
===

<!-- end_slide -->

## Container security

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Build-time controls**

Use minimal base images; pin versions; avoid `latest` tags.

Scan images during build and break on critical findings.

Run as non-root; drop unnecessary Linux capabilities.

Never bake secrets into images or layers.

<!-- column: 1 -->

**Runtime controls**

Read-only root filesystem where possible.

Restrict network egress and service-to-service access.

Set CPU and memory limits to reduce abuse blast radius.

Monitor for unusual process and network behaviour.

<!-- reset_layout -->

<!-- pause -->

Mental model — assume the container may be compromised. What can it still reach?

<!-- speaker_note: Ask "does anyone currently scan container images in CI?" — often the answer is no. The build-time scan is the easiest addition and catches the same categories as SCA but at the image level. -->

<!-- end_slide -->

## Cloud security — the three failure modes

Most cloud security incidents are configuration and identity mistakes, not zero-days.

<!-- pause -->

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**Too much access**
Over-permissive IAM roles — wildcard grants, roles shared across services.

Fix — least privilege, one role per workload.

<!-- column: 1 -->

**Too much exposure**
Public storage buckets, publicly accessible databases, broadly scoped secrets.

Fix — audit public resources; default to private.

<!-- column: 2 -->

**Too little visibility**
Disabled logs, no audit trail, alerts that nobody reads.

Fix — enable audit logging; route alerts to someone with a response process.

<!-- reset_layout -->

<!-- end_slide -->

## Identity and secrets in DevOps workflows

If CI is compromised, these controls determine the blast radius.

<!-- pause -->

<!-- incremental_lists: true -->

- Prefer short-lived credentials and federated identity — OIDC and workload identity over long-lived keys
- Keep secrets in a vault or managed secret store — not in the repo, not in CI variables where avoidable
- Rotate credentials automatically where possible
- Apply least privilege to CI runners and deployment identities — they do not need production database access

<!-- incremental_lists: false -->

<!-- pause -->

**Type in chat — does your CI runner have more access than it needs to deploy?**

<!-- speaker_note: Almost every team will answer yes if they think about it. CI runners with production database credentials, write access to all buckets, and admin IAM roles are extremely common. The credential-in-CI scenario from the opening is partly about this. -->

<!-- end_slide -->

## Infrastructure as code and policy

IaC changes are code changes — they need the same treatment.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**What this looks like**

Run IaC scanners (Terraform, Kubernetes manifests, cloud templates) in CI.

Review infrastructure PRs the same way as application code.

Encode guardrails as policy — no public databases, encryption required, specific allowed regions.

<!-- column: 1 -->

**Why it matters**

A Terraform change that opens a security group to `0.0.0.0/0` is as dangerous as a code change that removes an auth check.

Policies catch recurring mistakes before deployment and make the right thing the default thing.

<!-- reset_layout -->

<!-- end_slide -->

<!-- jump_to_middle -->

Monitoring and incident response
===

<!-- end_slide -->

## What should make a team suspicious

Detection only works if someone is watching for the right signals.

<!-- pause -->

<!-- incremental_lists: true -->

- Repeated login failures or unusual authentication patterns
- Sudden access to data across accounts or tenants — one user reading hundreds of records
- Unexpected outbound network calls from an application or container
- Security configuration changes nobody expected or requested
- Crash loops or unusual behaviour immediately after a deployment

<!-- incremental_lists: false -->

<!-- pause -->

Detection complements prevention. You need both because some things get through.

<!-- end_slide -->

## Incident response for developers

Five steps. The order matters.

<!-- pause -->

<!-- incremental_lists: true -->

1. **Detect and confirm** — is this real? What is the evidence? What is the severity?
2. **Triage** — scope and impact; which systems, accounts, or users are affected?
3. **Contain** — disable affected paths, revoke credentials, isolate affected services quickly
4. **Eradicate and recover** — patch, redeploy, rotate credentials, restore from clean state
5. **Learn** — post-incident review with concrete actions; update runbooks

<!-- incremental_lists: false -->

<!-- pause -->

A good incident response process is part of engineering quality, not a separate security concern.

Have runbooks before incidents, not during them.

<!-- end_slide -->

## Security runbook essentials

A runbook answers the questions a developer should not have to think about under pressure.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Clarity questions**

Who is on call and who has decision authority?

What logs and metrics do we check first?

What are the escalation thresholds — customer data exposure suspected, credential leak confirmed, production auth bypass observed?

<!-- column: 1 -->

**Action questions**

How do we quickly disable or isolate risky functionality?

How do we rotate credentials without breaking the service?

How do we communicate status internally and to affected customers?

<!-- reset_layout -->

<!-- pause -->

Fast response depends on clarity, not heroics.

<!-- end_slide -->

## Metrics that signal progress

Avoid vanity metrics. Measure whether the team is getting faster at making and resolving security decisions.

<!-- pause -->

| Metric | What it tells you |
|---|---|
| Time to triage | Detection and ownership speed |
| Time to remediate | How quickly risk is actually reduced |
| Repos with baseline controls | Rollout maturity |
| Exceptions past their SLA | Policy debt — known risk without a plan |

<!-- pause -->

"Number of scans run" is not a security metric. A scan nobody acts on is not coverage.

<!-- end_slide -->

<!-- jump_to_middle -->

Exercises
===

<!-- end_slide -->

## Exercise A — warn/block policy

You are setting pipeline security policy for a Node.js API. Five findings come in on the first scan.

**In pairs (5 minutes):** for each finding, decide block / warn / report only — and write one sentence justifying the decision.

<!-- pause -->

1. A JWT secret committed to the repo three months ago (now rotated)
2. A critical CVE in `lodash` (not called in any code path you can trace)
3. A high SAST finding — potential SQL injection in a rarely used admin endpoint
4. An expired TLS certificate on a staging endpoint
5. `npm audit` reports 47 moderate vulnerabilities in dev dependencies only

<!-- speaker_note: Push pairs to justify the edge cases — the lodash CVE that is not reachable and the rotated secret are the interesting decisions. The secret being rotated does not mean the finding should be suppressed — it means the credential risk is addressed but the process gap (why was it committed?) is not. -->

<!-- end_slide -->

## Exercise B — incident runbook

**In pairs (8 minutes):** create a minimum runbook for the scenario — "suspected credential leak from CI logs."

Your runbook must answer:

<!-- pause -->

1. What is the first action in the first 15 minutes?
2. How do you confirm whether the credential was actually used?
3. Who needs to be notified and when?
4. What is the definition of "contained" for this incident?
5. What process change would prevent a recurrence?

<!-- pause -->

One pair presents their runbook. Group challenges one decision.

<!-- speaker_note: The most common gap is question 2 — teams know to rotate but do not know how to check whether the credential was used before they rotated it. That requires log access and knowing what to look for. This connects back to the "too little visibility" cloud security failure mode. -->

<!-- end_slide -->

## Back to the credential in the pipeline

The repo was made public. The CI logs contained the database connection string. Your team has 60 minutes.

You said: rotate / check logs / notify / something else.

<!-- pause -->

The structured answer:

<!-- incremental_lists: true -->

1. **Contain first** — rotate the credential immediately; the database is exposed until you do
2. **Then investigate** — check database access logs for the window between the repo going public and the rotation; look for unexpected source IPs or queries
3. **Scope the exposure** — was the connection string the only secret in those logs?
4. **Communicate** — if customer data was accessible, that is a notification decision
5. **Fix the process** — credentials in CI variables that appear in logs is the gap; move to a vault or use federated identity

<!-- incremental_lists: false -->

<!-- pause -->

The 60-minute window is not just about the credential. It is about understanding what was reachable before you closed the door.

<!-- speaker_note: Return to the chat poll answers. Most will have said "rotate the credential" — correct, but incomplete. The investigation step is where many real incidents go wrong: teams rotate and close the incident without confirming whether the credential was used. -->

<!-- end_slide -->

## 30-day starter plan

Small, consistent steps beat a one-time security sprint.

<!-- pause -->

| Week | Action |
|---|---|
| Week 1 | Enable SCA and secrets scanning in CI |
| Week 2 | Write the warn/block policy; baseline existing findings |
| Week 3 | Add container image scanning and minimal hardening checks |
| Week 4 | Run one incident tabletop using the credential-leak scenario; capture actions |

<!-- pause -->

Each step builds on the previous one. Week 4 only works if weeks 1–3 have given you something to defend.

<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **Minimum viable pipeline** — secrets scanning, SCA, SAST, and a written warn/block policy
2. **Blast radius reduction** — container hardening, least-privilege IAM, short-lived credentials, vault for secrets
3. **Detection** — monitoring for the right signals; detection complements prevention
4. **Incident response** — contain first, then investigate, then recover; runbooks before incidents

<!-- end_slide -->

## Bridge to Module 8

**We've established:**

<!-- incremental_lists: true -->

- How to embed security through the pipeline
- How to reduce blast radius in containers and cloud
- How to respond consistently when something goes wrong

<!-- incremental_lists: false -->

**Module 8 — LLM Security:** prompt injection, data leakage, model misuse, and governance for AI-assisted development — the same DevSecOps thinking applied to AI features and AI tooling in your workflow.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===

*Introduction to Application Security — Module 7*