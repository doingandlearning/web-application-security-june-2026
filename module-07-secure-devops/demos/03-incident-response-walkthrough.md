# Demo 7.3 — Incident response walkthrough (developer-centred)

## Objective
Give a concrete, repeatable IR walkthrough that learners can reuse:
- detect & triage
- contain
- eradicate & recover
- learn + prevent recurrence

This demo is a tabletop-style walkthrough (no live breach needed).

---

## Scenario (use as your running story)

Production shows:
- repeated failed login attempts and unusual 403/401 patterns
- alerts suggesting that an API credential might have been exposed in logs or a public endpoint
- a spike in requests to a specific endpoint (e.g. `/api/reports` or `/api/tickets/search`)

Your team suspects:
- credential leak (possible session/auth bypass)
- potential sensitive data exposure

---

## What to show (step-by-step)

### Step 1: Detect & confirm
Ask: “What evidence do we check first?”
Suggested evidence:
- scanner/alert that triggered discovery
- time range of suspicious traffic
- which accounts/resources may be affected
- whether any secrets appear in logs or error messages

### Step 2: Triage
Decide:
- scope (which service(s), which environment)
- impact (what data/actions could be exposed)
- urgency (is this suspected credential leak vs confirmed?)

Output for triage:
- “We believe X is affected; we will treat it as [suspected/confirmed] for now.”

### Step 3: Contain
Choose containment actions that reduce damage quickly:
- disable affected endpoint/features behind a feature flag
- block attacker IPs / throttle / rate limit (if applicable)
- revoke/rotate credentials and invalidate sessions/tokens if leak is suspected

### Step 4: Eradicate & recover
- patch or change the vulnerable behavior (fix the root cause)
- redeploy
- rotate secrets again if needed (especially after redeploy)
- verify by running a quick functional/security check

### Step 5: Learn
Close the loop:
- update pipeline gates (e.g. secrets scanning, SCA, SAST baseline)
- add monitoring triggers that would have caught this earlier
- add/refresh runbooks and decision thresholds

---

## Talk track (key phrasing to land)

- “Contain first: we stop the bleeding while we figure out the fix.”
- “Fix the root cause, then re-run the checks so it doesn’t regress.”
- “The best incident response is engineering quality: runbooks + decision authority + shared process.”

---

## Ask the room (in chat/verbally)

1. “Which containment action would you do first: disable feature, rotate secrets, revoke tokens, block IPs?”
2. “If you can’t confirm exposure yet, do you still rotate? Why/why not?”
3. “What pipeline check would most likely have prevented the incident from shipping?”

---

## Suggested demo outcome

By the end, learners should be able to fill a mini runbook with:
- immediate containment
- who decides/escalates
- what ‘success’ looks like after recovery
