# Lab 7.4 — Map OWASP WSTG to your pipeline

## Objective

The [OWASP Web Security Testing Guide (WSTG)](https://owasp.org/www-project-web-security-testing-guide/) is a **methodology and checklist library** — not something you install and run. It tells you *what* to test and *how*.

Your team already has tools (secret scanning, SCA, SAST, ZAP). This exercise turns the WSTG into the **test-case backlog** behind those tools: each checklist item becomes a test plan entry, mapped to an SDLC phase and a gate in your pipeline.

You will:

1. Map the WSTG five-phase testing framework to activities and tools for TrustyTickets
2. Assign course vulnerabilities to the phase where they are cheapest to catch
3. Use WSTG categories as a rubric to interpret scanner output and decide pipeline action

Allow **20–25 minutes** plus debrief. Works solo or in pairs.

---

## Background — the five WSTG phases

The OWASP Testing Framework (WSTG Section 3) places security testing across the lifecycle — not only at the end:

| Phase | Focus |
|-------|--------|
| **1 — Before development** | SDLC defined; secure coding standards; metrics decided |
| **2 — Definition and design** | Security requirements review; architecture review; threat modelling |
| **3 — During development** | Code walkthroughs; static review against checklists; SAST |
| **4 — During deployment** | Penetration testing; configuration review; DAST on a running app |
| **5 — Maintenance and operations** | Periodic health checks; verify changes have not reintroduced fixed issues |

Reference: [OWASP WSTG — The OWASP Testing Framework](https://owasp.org/www-project-web-security-testing-guide/latest/3-The_OWASP_Testing_Framework/)

---

## Task 1: Map phases to TrustyTickets (10 minutes)

For each WSTG phase, write **one concrete activity** your team would do on TrustyTickets and **one tool or artefact** that supports it.

Use tools from this course where they fit — threat sketches, PR checklists, Roslyn/SAST, `dotnet list package --vulnerable`, secret scanning, ZAP baseline, regression tests, runbooks.

| WSTG phase | One activity on TrustyTickets | Tool or artefact |
|------------|------------------------------|------------------|
| 1 — Before development | | |
| 2 — Definition and design | | |
| 3 — During development | | |
| 4 — During deployment | | |
| 5 — Maintenance and operations | | |

**Hints:**

- Phase 1 is the one teams skip most often — think standards and policy, not scanning.
- Phase 2 is the cheapest place to catch a bad auth or validation design — you did threat modelling in Module 2.
- Phase 3 is where SAST and manual review against OWASP-style checklists live.
- Phase 4 is where ZAP and configuration checks belong — issues that only appear on a running app.

---

## Task 2: Where should we have caught it? (8 minutes)

You already exploited and fixed these in TrustyTickets. For each, fill in:

- **Cheapest WSTG phase** to catch it (1–5)
- **Primary control** — design review, code review, SAST rule, DAST scan, regression test, or ops check
- **One sentence** — why that phase is cheaper than finding it in production

| Vulnerability | Cheapest phase (1–5) | Primary control | Why this phase |
|---------------|---------------------|-----------------|----------------|
| SQL injection in ticket search | | | |
| IDOR on `GET /api/tickets/{id}` | | | |
| Stored XSS in comments | | | |
| Missing security headers (CSP, etc.) | | | |

**Do not** answer “run ZAP” for everything. Some of these should never reach a DAST scan.

---

## Task 3: WSTG rubric for scanner output (7 minutes)

ZAP baseline scan against TrustyTickets returned these alerts. WSTG Section 4 ([Web Application Security Testing](https://owasp.org/www-project-web-security-testing-guide/latest/4-Web_Application_Security_Testing/)) groups tests by area — use it as a rubric, not just the ZAP risk label.

For each alert:

1. **WSTG area** — e.g. input validation, session management, configuration, authentication, error handling
2. **True positive or likely false positive?** — one sentence
3. **Pipeline action** — `BLOCK` / `WARN + TICKET` / `REPORT ONLY` (use your Task 1 policy from Lab 7.1 if you have one)
4. **Earlier phase?** — which WSTG phase (1–5) could have prevented or caught this before deploy?

---

**Alert A**

```
Risk: High
Name: SQL Injection
URL: GET /api/tickets?search=...
Evidence: database error / unexpected rows in response
```

---

**Alert B**

```
Risk: Medium
Name: Content Security Policy (CSP) Header Not Set
URL: all pages
Evidence: no Content-Security-Policy response header
```

---

**Alert C**

```
Risk: Low
Name: X-Powered-By header leak
URL: all responses
Evidence: X-Powered-By: ASP.NET
```

---

## Debrief (chat share)

Type in chat: **Alert A — which WSTG phase should have caught this first, and with what control? One sentence.**

The facilitator will read responses. Push the room to name a phase *before* deployment and a specific control — not just "run ZAP again."

---

## Expected output

- Task 1: completed five-row mapping table
- Task 2: completed four-row “cheapest phase” table
- Task 3: WSTG category + triage decision for all three alerts

---

## Key concepts

- **WSTG is the what; your pipeline is the when** — checklist items become test cases slotted into phases and tools.
- **Shift left has a reference model** — Phase 2 design flaws cost a conversation; Phase 4 findings cost a deploy cycle.
- **Scanner output needs a rubric** — WSTG categories help you interpret alerts and ask “should we have caught this earlier?”

---

## Next steps

- Pull specific WSTG checklist items (e.g. SQL injection, CSRF, session management) into your team's test plan or ZAP scan policy.
- See [WSTG Appendix A — Testing Tools Resource](https://owasp.org/www-project-web-security-testing-guide/latest/Appendix_A-Testing_Tools_Resource/) for tool-to-checklist mappings.
- Module 8 applies the same “policy + verification” mindset to LLM-assisted development.
