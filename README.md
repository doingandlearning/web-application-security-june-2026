# Introduction to Application Security

Course materials for **Introduction to Application Security** — threats, secure SDLC,
OWASP Top 10, secure coding, session and data protection, security testing, DevSecOps,
and LLM security.

---

## Timings

|---|---|
| 09:30 – 11:00 | Session 1 |
| 11:00 – 11:20 | Break |
| 11:20 – 12:45 | Session 2 |
| 12:45 – 13:45 | Lunch |
| 13:45 – 15:15 | Session 3 |
| 15:15 – 15:35 | Break |
| 15:35 – 16:30 | Session 4 |

---

## Lab app — TrustyTickets

**[TrustyTickets](TrustyTickets/)** is an intentionally vulnerable ASP.NET Core (C#)
and vanilla JS ticketing app. It is the hands-on target for Modules 3, 4, 5, and 6.

Start it before Module 3:

```bash
cd TrustyTickets && dotnet run
```

Full setup instructions in `TrustyTickets/README.md`.

Vulnerabilities in scope: SQLi, IDOR, XSS, CSRF, auth issues, missing security headers.

---

## Modules

| # | Title | Slides | Exercises |
|---|---|---|---|
| 1 | Introduction to Application Security | `module-01/slides.md` | `module-01/exercises/` |
| 2 | Secure SDLC | `module-02/slides.md` | `module-02/exercises/` |
| 3 | Common Vulnerabilities | `module-03/slides.md` | `module-03/exercises/` |
| 4 | Input Validation and Secure Coding | `module-04/slides.md` | `module-04/exercises/` |
| 5 | Session Management and Data Protection | `module-05/slides.md` | `module-05/exercises/` |
| 6 | Security Testing | `module-06/slides.md` | `module-06/exercises/` |
| 7 | Secure DevOps | `module-07/slides.md` | `module-07/exercises/` |
| 8 | LLM and AI Security | `module-08/slides.md` | `module-08/exercises/` |
| 9 | Summary and Next Steps | `module-09/slides.md` | — |