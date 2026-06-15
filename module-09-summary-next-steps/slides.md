---
title: "**Introduction to Application Security**"
sub_title: Module 9 — Summary and Next Steps
author: Kevin Cunningham
---

## Where we started

```
GET /api/ticket?id=142
```

In Module 1 you were asked — bug, vulnerability, both, or neither?

<!-- pause -->

You now know the answer precisely — broken access control, OWASP A01, missing ownership check, one line of authorisation logic to fix.

<!-- pause -->

More importantly, you know what else to ask.

Is it in the SDLC? Would SAST have caught it? Would your pipeline have blocked it? Would your runbook cover the incident if it was exploited?

<!-- speaker_note: Pause after "more importantly, you know what else to ask." Let that land before moving to the questions. This is the thread that connects every module back to a single concrete example. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Course in review
===

<!-- end_slide -->

## Eight modules, one thread

Every module returned to the same question — not "is this secure?" but "what is the blast radius when something goes wrong, and how quickly can you close it?"

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Modules 1–3 — Understand**

Attack surface, trust boundaries, and the SDLC gave you the frame.

Hands-on exploitation made the failure modes concrete rather than theoretical.

<!-- column: 1 -->

**Modules 4–5 — Fix**

Secure coding patterns, input validation, session management, and data protection gave you the specific changes.

<!-- reset_layout -->

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Module 6 — Find**

SAST, DAST, SCA, and pen test fundamentals gave you the tools to catch what you missed.

<!-- column: 1 -->

**Modules 7–8 — Sustain**

DevSecOps automation and LLM security moved security from a gate to a habit.

<!-- reset_layout -->

<!-- end_slide -->

## Key principles that cut across every module

<!-- incremental_lists: true -->

- **Attack surface thinking** — every input is a potential entry point; every trust boundary needs a check
- **Shift left** — finding things at design costs a conversation; finding them in production costs an incident
- **Blast radius reduction** — assume something gets through; limit what it can reach
- **Trust but verify** — applies to code review, tool output, and LLM suggestions equally
- **Automation makes "we checked" repeatable** — one manual review is not a process

<!-- incremental_lists: false -->

<!-- end_slide -->

<!-- jump_to_middle -->

Incident response
===

<!-- end_slide -->

## The loop you can run from memory

Five steps. Applicable to a leaked credential, an exploited vulnerability, or a misconfigured cloud resource.

<!-- pause -->

<!-- incremental_lists: true -->

1. **Detect and confirm** — is this real? What is the evidence? What is the severity?
2. **Triage** — scope and impact; which systems, accounts, or users are affected?
3. **Contain** — disable affected paths, revoke credentials, isolate affected services — do this first
4. **Eradicate and recover** — patch, redeploy, rotate credentials, restore from a clean state
5. **Learn** — post-incident review with concrete actions; update runbooks so the next response is faster

<!-- incremental_lists: false -->

<!-- pause -->

Contain before you investigate. Investigate before you recover. The order matters.

<!-- end_slide -->

<!-- jump_to_middle -->

Your next steps
===

<!-- end_slide -->

## Personal plan — next two weeks

Three specific commitments. Write them down now.

<!-- pause -->

**1. One security check to enable**

SCA on your current project. A SAST rule on a sensitive code path. A ZAP baseline scan against a staging environment.

Something you can do in a day and that will catch a real category of risk.

<!-- pause -->

**2. One secure-coding habit to enforce in PRs**

A checklist item — ownership check on every endpoint that returns user data, parameterised queries only, `HttpOnly` on session cookies.

Something your team can verify in code review without specialist knowledge.

<!-- pause -->

**3. One LLM red line**

A clear rule for your team — what you will not paste into a public model, what categories of generated code always need manual review, how you handle AI-suggested security code.

<!-- end_slide -->

## Team plan — next 30 to 60 days

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Week 1**
Enable SCA and secrets scanning in CI.

**Week 2**
Write the warn/block policy. Baseline existing findings — fail on new issues only.

<!-- column: 1 -->

**Week 3**
Add container image scanning and minimal hardening checks.

**Week 4**
Run one incident tabletop. Use the credential-leak scenario from Module 7. Capture concrete actions.

<!-- reset_layout -->

<!-- pause -->

The tabletop in week 4 only works if weeks 1–3 have given you something to defend. Start with SCA.

<!-- end_slide -->

## Exercise — commit to one thing

Take five minutes. Write down one answer to each question.

<!-- pause -->

**In the next two weeks:**
What one security check will you enable or run? Be specific — name the tool, the project, and when.

<!-- pause -->

**In the next 30 to 60 days:**
What one team process will you change? Name the change, who needs to agree, and what done looks like.

<!-- pause -->

**Share one with the group** — not as a goal, as a commitment. Name the action and the date.

<!-- speaker_note: Give five minutes of genuine silence. Resist the urge to fill it. The specificity of the commitment is what makes it stick — "I will add npm audit to the PR pipeline on the payments service by Friday" is a commitment. "I will try to do more security" is not. -->

<!-- end_slide -->

## Resources for further learning

**OWASP**

- OWASP Top 10 — owasp.org/www-project-top-ten
- OWASP LLM Top 10 — owasp.org/www-project-top-10-for-large-language-model-applications
- OWASP Cheat Sheet Series — cheatsheetseries.owasp.org

<!-- pause -->

**Tools**

- OWASP ZAP — zaproxy.org
- Burp Suite — portswigger.net/burp
- Snyk — snyk.io

<!-- pause -->

**Your most useful next step**

The lab repo and exercise sheets from this course. Run the exploits again. Fix them yourself. Then add a test that catches the regression.

<!-- speaker_note: The last point is the most important one. Everything in this course was designed to be revisited with the code in front of you. The delegates who will retain this material are the ones who go back to TrustyTickets next week and try to break something new. -->

<!-- end_slide -->

## The ticket is still there

Somewhere in a codebase you work on, there is an endpoint that looks like this:

```
GET /api/ticket?id=142
```

Maybe it has an ownership check. Maybe it does not. Maybe it did last month.

<!-- pause -->

The work is not a one-time fix. It is a habit — in code review, in design, in the pipeline, and in incident response.

<!-- pause -->

You now have the vocabulary, the tools, and the patterns to do that work.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===

*Introduction to Application Security — Module 9*