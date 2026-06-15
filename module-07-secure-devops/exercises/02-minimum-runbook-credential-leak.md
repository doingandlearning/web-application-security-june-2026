# Lab 7.2 — Minimum runbook: suspected credential leak

## Objective
Create a small, usable incident response runbook that developers can follow. The focus is on **who does what, what to do first, and how to prevent recurrence**.

You will produce:
- a 1-page runbook (in your own words)
- an escalation trigger list

---

## Scenario

Your pipeline or monitoring flags a possible credential leak:
- a secret scanner detected something that looks like an API key in a commit or build log
- you also see suspicious 401/403 patterns after the commit was deployed

You do NOT have full confirmation yet, but you must act as if this could be real until proven otherwise.

---

## Task 1: Fill the runbook structure

Use the sections below. Write short bullet steps.

### A) Trigger + evidence (Detect & confirm)
- What triggers this runbook?
- What evidence do you check in the first 10–20 minutes?
- What does “suspected” vs “confirmed” mean in your runbook?

### B) Triage (scope + impact + urgency)
- Which systems/environments might be affected?
- What data/actions might be exposed?
- Who needs to be involved (Dev, Sec, Ops)?

### C) Contain (stop the bleeding)
Pick 2–4 containment actions. Examples:
- disable the affected feature/endpoint behind a flag
- block/throttle suspicious traffic (rate limit / WAF)
- revoke/rotate credentials and invalidate sessions/tokens

For each action, add:
- “why this first”
- who executes it

### D) Eradicate & recover (fix + redeploy)
- What is the root-cause fix you expect to deploy?
- What rotation/redeploy steps happen after the patch?
- How do you verify recovery (quick functional checks)?

### E) Learn (prevent recurrence)
- what pipeline checks should be tightened (secret scan, baseline policy, SCA)?
- what monitoring triggers should be added/adjusted?
- update runbook with what you learned

---

## Task 2: Add escalation thresholds

Write 3 escalation triggers in this style:

- Escalate when: ________
- Escalate when: ________
- Escalate when: ________

Use examples like:
- suspected credential leak becomes confirmed
- evidence of access to customer data
- authentication bypass observed

---

## Task 3: Validate it with a 2-minute rehearsal

In pairs, read your runbook and answer:
- What’s your fastest first action?
- What information do you need before containment is safe?
- Who has decision authority?

---

## Example output

1. A completed runbook with the 5 sections (A–E)
2. A list of 3 escalation triggers

---

## Key concepts demonstrated

- Incident response is an engineering workflow, not just security governance
- Contain first; fix the root cause after you stop damage
- Post-incident learning improves pipeline + monitoring so regressions don’t recur

