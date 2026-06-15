---
title: "**Introduction to Application Security**"
sub_title: Module 6 — Security Testing
author: Kevin Cunningham
---

## The scan came back clean

Your team adds OWASP ZAP to the CI pipeline. The baseline scan runs on every deploy. No high-severity findings.

Three weeks later a pen tester finds SQL injection in the search endpoint.

<!-- pause -->

**Type in chat — how is that possible?**

We'll come back to this at the end of the module.

<!-- speaker_note: Common answers — the scanner missed it, the scanner was not configured correctly, the endpoint needed authentication. All plausible. Hold the tension and let the module build toward the answer. The key insight is that scanners find candidates, not exhaustive coverage, and different tools see different things. -->

<!-- end_slide -->

<!-- jump_to_middle -->

The testing landscape
===

<!-- end_slide -->

## Three lenses on security

No single tool finds everything. Each sees a different part of the picture.

<!-- pause -->

| | SAST | DAST | SCA |
|---|---|---|---|
| What it analyses | Source code or binary | Running application | Dependencies and packages |
| Finds vulnerable code paths | Yes | No | No |
| Finds runtime misconfigurations | No | Yes | No |
| Finds known CVEs in libraries | No | No | Yes |
| Needs the app to be running | No | Yes | No |

<!-- pause -->

**Type in chat — which of these three does your team currently run?**

<!-- speaker_note: Most teams run SCA (often via Dependabot without realising it counts). SAST is less common. DAST in CI is rare. Use the answers to frame which gaps the rest of the module addresses. -->

<!-- end_slide -->

## ZAP and Burp — two different jobs

Both are DAST tools. They are not interchangeable.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**OWASP ZAP**
Free and open source. Scriptable and headless. Designed to run in CI — baseline scans on every deploy.

Best for: automated coverage, quick feedback on every change.

<!-- column: 1 -->

**Burp Suite**
Proxy, Repeater, Intruder, scanner. Designed for interactive manual testing.

Best for: complex authentication flows, business logic, anything automation cannot guess.

<!-- reset_layout -->

<!-- pause -->

Rule of thumb — ZAP for automation, Burp for investigation. Most professional testing uses both.

<!-- end_slide -->

<!-- jump_to_middle -->

ZAP in practice
===

<!-- end_slide -->

## Demo — ZAP baseline scan

**Demo:** start TrustyTickets, point ZAP at the app URL, run a baseline scan.

Show the Alerts panel. Open one finding — read the request, the response, and the risk rating.

<!-- speaker_note: Run this live. Walk through one alert in detail — show the HTTP request ZAP sent and what response triggered the flag. Ask "is this a real vulnerability or a false positive?" before answering. The messiness of real scanner output is the point — learners need to see that findings require interpretation. -->

<!-- end_slide -->

## Reading a ZAP finding

Every alert has the same structure. Use it to triage, not just to read.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**What the alert tells you**

- Risk level — High / Medium / Low / Informational
- The request ZAP sent
- The response that triggered the flag
- The pattern it matched

<!-- column: 1 -->

**What you need to decide**

- Can you reproduce it manually?
- Does it apply to your app's context?
- Is it a true positive or false positive?

<!-- reset_layout -->

<!-- pause -->

A scanner finding is a hypothesis. Reproduction is the test.

<!-- end_slide -->

## Triage process

Four steps — every finding, every tool.

<!-- pause -->

<!-- incremental_lists: true -->

1. **Reproduce** — follow the scanner's request and response; confirm the behaviour yourself
2. **Decide** — true positive (real vulnerability) or false positive (scanner pattern matched but no real risk)?
3. **Act** — true positive: fix or accept with written justification; false positive: document and suppress in tooling
4. **Baseline** — configure the build to fail only on new critical or high findings, not on known-and-accepted ones

<!-- incremental_lists: false -->

<!-- pause -->

If everything is critical, nothing is. Tuning takes time — start with warnings, move to gates once you trust the signal.

<!-- speaker_note: Connect back to Module 2 — this is the "tuning and gates" conversation from the pipeline section. Learners who were sceptical about alert fatigue in Module 2 will recognise it here with a concrete example. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Burp Suite
===

<!-- end_slide -->

## Demo — Burp Repeater

**Demo:** configure the browser to proxy via Burp. Log in and click around TrustyTickets. Find an interesting request in the history. Send it to Repeater. Change a parameter. Resend.

<!-- speaker_note: Connect this explicitly to Module 3. "This is how we tested for IDOR manually — change the ticket ID in Repeater and see what comes back." The key insight is that Burp makes manual testing repeatable and precise in a way that browser devtools alone do not. If time is short, skip the Burp demo and refer delegates to the Burp Academy for self-study. -->

<!-- end_slide -->

## What Burp does that ZAP cannot

Automated scanners cannot guess business logic. Burp is for the cases automation misses.

<!-- pause -->

<!-- incremental_lists: true -->

- Multi-step flows — an attack that only works after a specific sequence of requests
- Context-dependent payloads — injection that only triggers under certain user roles or states
- Auth bypass — testing what happens when you replay a request with a different session token
- IDOR at scale — Intruder to enumerate a range of IDs and spot which return unexpected data

<!-- incremental_lists: false -->

<!-- pause -->

This is also what pen testers do — at scale, with more time, and with a brief to find everything.

<!-- end_slide -->

<!-- jump_to_middle -->

SAST and SCA
===

<!-- end_slide -->

## SAST — security in the IDE and PR

Static analysis scans code without running it. For a developer, it usually appears as a PR comment or IDE warning.

<!-- pause -->

<!-- column_layout: [2, 3] -->

<!-- column: 0 -->

**What it finds**

Concatenated SQL, unsafe API calls, tainted input reaching a sink, hardcoded secrets, unsafe deserialization.

**What it misses**

Runtime configuration, business logic flaws, anything that only appears at execution time.

<!-- column: 1 -->

**How to use a SAST finding**

1. Read the flagged line
2. Ask — can user input reach this code path?
3. If yes — apply the fix from Module 4
4. If no — document and suppress

<!-- reset_layout -->

<!-- end_slide -->

## SCA — dependency scanning

Scans your package manifest for libraries with known CVEs. The easiest security gate to add and one of the highest signal.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Tools**
Dependabot (GitHub), Snyk, `npm audit`, `dotnet list package --vulnerable`

**Where it runs**
On every PR or on a schedule — does not need the app to run.

<!-- column: 1 -->

**What a finding looks like**
Package X version Y has CVE-YYYY-NNNNN, severity High. Upgrade to version Z.

**What to do**
Upgrade and test. If upgrade breaks the build, assess exploitability before accepting the risk.

<!-- reset_layout -->

<!-- pause -->

If your team runs nothing else, run SCA. It is low effort, high return, and catches a category of risk that SAST and DAST cannot see.

<!-- end_slide -->

<!-- jump_to_middle -->

Pen testing fundamentals
===

<!-- end_slide -->

## What a pen test is — and your role in it

Authorised, scoped, time-boxed testing of your application by someone with an explicit brief to find vulnerabilities.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**What you get back**

A report: each finding has a title, risk rating, description, steps to reproduce, and a recommended fix.

<!-- column: 1 -->

**Your role**

Reproduce the finding. Understand the root cause. Apply the fix — and write a test that would have caught it.

<!-- reset_layout -->

<!-- pause -->

Pen testers do the same things we did in Module 3 — inject, change IDs, test authorisation — but at scale, with more time, and with no prior knowledge of the codebase.

<!-- end_slide -->

## Reading a pen test finding

**Title:** IDOR on `/api/tickets/{id}`
**Risk:** High

<!-- pause -->

**Description:** an authenticated user can view any ticket by supplying an arbitrary ID. No ownership check is performed server-side.

**Steps to reproduce:**
1. Log in as User A
2. Note a ticket ID visible in the UI
3. Request `/api/tickets/{id}` with a different ID — one belonging to User B
4. Observe that User B's ticket is returned in full

<!-- pause -->

**Fix:** add an ownership check before returning the resource. This was covered in Module 4 — the pattern is the same.

<!-- speaker_note: Walk through this finding as if reading a real report. The point is that the format is consistent and learnable. Ask "what test would catch a regression here?" — a test that authenticates as User A and asserts a 403 on User B's ticket. -->

<!-- end_slide -->

## Exercise — ZAP scan and triage

Run a ZAP baseline scan against TrustyTickets (or use the pre-captured results in `exercises/`).

**In pairs (10 minutes):**

<!-- pause -->

1. Pick **one High or Medium** finding from the Alerts panel
2. Reproduce it — follow the request ZAP sent and confirm the behaviour manually
3. Decide — true positive or false positive?
4. Write one sentence — "To fix this I would…" or "This is a false positive because…"

<!-- pause -->

Bring your finding and your sentence back to the group.

<!-- speaker_note: Circulate and prompt. Common issues — pairs pick Informational findings which are hard to reproduce. Redirect them to High or Medium. The "true positive or false positive" decision is the critical step — push pairs to justify, not just guess. -->

<!-- end_slide -->

## Back to the clean scan

ZAP ran on every deploy. No high-severity findings. A pen tester still found SQL injection.

You said: **how is that possible?**

<!-- pause -->

DAST tools send unauthenticated or lightly authenticated requests by default. A search endpoint behind login may never be tested.

<!-- pause -->

ZAP sees the surface the scanner can reach. The pen tester authenticated, explored the application as a real user, and tested the search endpoint specifically.

<!-- pause -->

**A clean scan means "no findings in scope." It does not mean "secure."**

The fix is not a better scanner. It is authenticated scanning, SAST on the search code, and coverage of the right endpoints.

<!-- speaker_note: This is the answer to the opening provocation. Scanners have coverage limits — authentication, dynamic flows, business logic. The lesson is that tools are complementary. None of them replaces the others. -->

<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **SAST, DAST, SCA** see different things — code paths, running behaviour, dependency CVEs; use at least two
2. **ZAP** for automated baseline scans in CI; **Burp** for manual investigation of complex flows
3. **Triage** — every finding needs reproduction and a true/false positive decision before action
4. **Pen tests** produce findings in the same format as the vulnerabilities from Module 3; your job is to reproduce, fix, and prevent regression

<!-- end_slide -->

## Bridge to Module 7

**We've established:**

<!-- incremental_lists: true -->

- Which tools to run and what each one sees
- How to triage findings and tune gates
- How security testing connects to the fixes from Modules 4 and 5

<!-- incremental_lists: false -->

**Module 7 — Secure DevOps:** wiring SAST, DAST, and SCA into the pipeline; container and cloud security; incident response and monitoring.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===

*Introduction to Application Security — Module 6*