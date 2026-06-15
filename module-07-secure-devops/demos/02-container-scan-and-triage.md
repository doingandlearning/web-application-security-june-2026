# Demo 7.2 — Container image scanning + interpreting results

## Objective
Demonstrate:
- how container scanning fits into the pipeline (scan after build)
- how to interpret results (CVE vs secrets vs misconfiguration)
- how to decide **block vs warn** using the course’s policy

This demo can be done with sample output (no live tools required).

---

## What to show (live)

1. A “bad habit” Dockerfile snippet
2. Sample scanner output (Trivy/Snyk/registry-native-style)
3. A triage mapping:
   - critical/high CVEs in the image → block
   - medium findings → warn/ticket
   - info/low findings → report/baseline

---

## Example: a deliberately insecure Dockerfile snippet (conceptual)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:latest

# BAD: secrets baked into the image layers (for teaching/lab)
COPY appsettings.json /app/appsettings.json

# BAD (teaching): runs as root (default)
# (No USER directive)

WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "TrustyTickets.dll"]
```

---

## Example scanner output (simplified)

```text
Scanning image: myorg/trustytickets:1.0

Vulnerabilities:
  CRITICAL  CVE-2023-XXXX  openssl 3.x  (fix available)
  HIGH      CVE-2022-YYYY  libcurl   (fix available)
  MEDIUM    CVE-2021-ZZZZ  busybox   (no fix available in base)

Secrets:
  HIGH      Detected in appsettings.json: "API_KEY=sk_live_..."

Misconfiguration:
  WARNING   Image runs as root (USER not set)
```

---

## Talk track (what to say)

- “Containers are part of the delivery artifact. Scan them like you scan dependencies.”
- “A secure image doesn’t help if runtime config is loose.”
- “The policy is the important part: tools will produce noise; we decide based on severity + context.”

---

## Ask the room (fast triage questions)

1. “Would you block the build due to the detected secret?”
2. “Do you block on CRITICAL/HIGH CVEs even if the app might not hit that code path?”
3. “For MEDIUM CVEs with no fix in base: warn, ticket, or accept?”
4. “Which runtime controls would reduce blast radius if the container is compromised?”

---

## Suggested demo outcome

End with a one-paragraph “policy decision” summary:
- Block on secrets + critical/high image CVEs
- Warn on medium findings and open a ticket with owner + due date
- Treat root/non-root + network egress as part of the hardening checklist (blast radius)

---

## Optional tie-back to incident response
- “If we miss a high-risk container vuln, monitoring should still detect weird runtime behaviour early.”
