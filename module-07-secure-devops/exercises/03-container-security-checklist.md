# Lab 7.3 (Optional) — Container security checklist

## Objective
Practice turning container security advice into a concrete checklist for build-time and runtime controls.

You will:
1. Inspect a sample Dockerfile and running assumptions
2. Identify build-time and runtime weaknesses
3. Write 3 fixes you would ask for before deploying

---

## Scenario

Your team is containerising a web app. A scan found:
- known vulnerabilities in the base image
- the container appears to run as root
- secrets may be baked into the image layers

You don’t need to run the container scan tool in this lab. You need to make good security decisions based on the evidence and best practices.

---

## Task 1: Review a sample Dockerfile (find issues)

Examine the intentionally flawed snippet:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:latest

# BAD (teaching): secret baked into image layers
COPY appsettings.json /app/appsettings.json

WORKDIR /app
COPY . .

# BAD: no USER directive (assumes root)
ENTRYPOINT ["dotnet", "TrustyTickets.dll"]
```

Write down:
- 2 build-time issues
- 1 issue related to secrets/credential handling
- 1 issue related to runtime containment (non-root, blast radius)

---

## Task 2: Convert issues to a checklist (3 bullets)

Produce exactly 3 “asks” for the delivery team:
- One for base image management
- One for secrets handling
- One for runtime containment / blast radius

Format:
- `Ask 1:` ________
- `Ask 2:` ________
- `Ask 3:` ________

---

## Task 3: Map to pipeline placement

Where in the pipeline would you enforce each ask?
- `Build / package:` ________
- `Deploy / run:` ________

---

## Example output

A 3-bullet checklist plus where each check belongs in the pipeline.

---

## Key concepts demonstrated

- A secure image isn’t enough: runtime config matters
- Container security reduces blast radius when something is compromised
- Build-time controls and runtime controls should be treated separately

