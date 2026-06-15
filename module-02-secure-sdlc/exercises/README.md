# Module 2 — Exercises

## Lab 2: Security in the pipeline and threat sketching

## Scenario: Your team's pipeline and system

You are working on a service that ships regularly. It has a typical pipeline — commits, pull requests, a build, tests, and deployments to test and production environments. The system itself is similar to TrustyTickets — a web app, a backend API, and a database, with some external integrations.

Use your own real pipeline and system wherever that makes the discussion more concrete.

---

## Task 1: Map your pipeline and add one security check (10–15 minutes)

### Step 1: Individual (3–5 minutes)

Sketch your current or typical pipeline:

```
developer → commit → pull request → build → test → deploy (staging / prod)
```

Include any manual sign-offs or approvals. Mark any existing security-relevant steps — dependency scanning, code review checklists, a security review gate, anything that relates to security even informally.

If nothing is automated yet, that is useful information — sketch what should happen when code moves from laptop to production.

### Step 2: Chat poll

Type in chat: **"Does your team run any automated security checks today? Yes / No / Not sure"**

### Step 3: Plenary (5–8 minutes)

Share one of the following — type it in chat or unmute briefly:

- Where in your pipeline would you add a dependency scan?
- Where would you add a secret scan?
- Where would SAST fit — on every PR, or nightly?

The facilitator will build a composite pipeline on the shared board showing where checks land across the group.

**Hints:**
- Checks are most effective close to where risk is introduced.
- Dependency scan and secret scan on PR are the lowest-friction starting points.
- SAST on every PR catches more but requires tuning; nightly is a reasonable starting point.

--

## Task 2: Threat sketch (10–15 minutes)

### Step 1: Individual (5 minutes)

Take the TrustyTickets system — or your own app — and list **three threats**, one sentence each.

Use the pattern: "An attacker could [do X] by [leveraging Y], which would [cause Z]."

For each threat, rate likelihood and impact as High / Medium / Low. Pick the one you would fix first and note why.

**Hints:**
- Use STRIDE as a prompt if you get stuck — Spoofing, Tampering, Repudiation, Information disclosure, Denial of service, Elevation of privilege.
- Focus on your actual system, not textbook examples.
- The goal is the habit of comparing threats, not a perfect risk matrix.

<details>
<summary>Example threats</summary>

```
1. An attacker could read other users' tickets by changing the ID in the API request,
   because there is no ownership check — impact: data exposure, likelihood: high.

2. An attacker could inject SQL via the search box to extract the full tickets table,
   because input is concatenated into the query — impact: high, likelihood: medium.

3. An attacker could post a script in a comment that runs in other users' browsers,
   because comments are rendered with innerHTML — impact: medium, likelihood: medium.

Fix first: threat 1 — high likelihood, easy to exploit, directly exposes customer data.
```

</details>

### Step 2: Chat share

Type your top-priority threat in chat — one sentence.

The facilitator will group responses by theme (injection, access control, auth/session, data exposure) and ask: did different people pick the same threat? If not — why not? What assumptions led to different priorities?

---

## Task 3: First security gate (10 minutes)

### Step 1: Individual decision (3 minutes)

Your team can add one automated security check to the pipeline this sprint. Choose one:

- **Dependency scan on every PR** — blocks merge if a critical CVE is found in a runtime dependency
- **Secret scan on commit** — blocks push if a key pattern is detected
- **SAST on build** — warns on high-severity findings, does not block

Write down:
- Which you chose
- One reason in favour
- One objection a sceptical senior developer might raise, and how you would answer it

### Step 2: Chat poll

Type in chat: **"Dependency scan / Secret scan / SAST"**

### Step 3: Plenary (5 minutes)

The facilitator will call on one person who chose each option to give their reasoning and anticipated objection. Group discusses: is the objection valid? What would change the answer?

**Hints:**
- Secret scan tends to have the lowest false-positive rate and the highest immediate impact — easiest to justify.
- Dependency scan is high-value but requires a policy decision on which severities block.
- SAST produces the most noise on first run — start with warnings and a baseline.

<details>
<summary>Policy sentence examples</summary>

```
"Run npm audit on every PR; block merge on critical or high findings in runtime dependencies."
"Run secret scan on every push; block the push if a key pattern is detected anywhere in the diff."
"Run SAST on every build; warn on high findings; fail only on new critical findings after baseline."
```

</details>

---

## Expected output

- A composite pipeline diagram on the shared board showing where security checks land.
- A grouped list of threats from across the group, sorted by theme.
- A chat poll result showing which gate the group would start with, and the key objection for each.

---

## Key concepts

- **Shift left** — security checks are more effective close to where risk is introduced.
- **Threat modelling** — a sketch and three questions is enough to start; STRIDE gives you prompts when you stall.
- **Warn vs block** — tuning gates requires a written policy; without one, tools produce noise and teams route around them.

---

## Next steps

You will use the pipeline map again in Module 7 (DevSecOps) when you wire up concrete tools. Keep your threat list — the same threats appear in Module 3 as hands-on exploitation targets.