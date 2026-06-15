# News stories collection (validated links)

Validated all links (including additions) by fetching them successfully on `2026-03-20`.

## LLM / AI security stories (Module 8)

1. [OpenAI ChatGPT “Command Memories” Injection via SearchGPT (Tenable TRA-2025-11)](https://www.tenable.com/security/research/tra-2025-11)
  - What happened: indirect prompt injection via browsing/summarisation can inject and exfiltrate “Command Memories” (potentially including PII).
  - Skills that would help: `LLM01` prompt injection defenses; `LLM06` sensitive data handling (“when in doubt, don’t send”); trust-but-verify and output handling.
2. [Zero-click AI data leak flaw uncovered in Microsoft 365 Copilot (EchoLeak)](https://www.bleepingcomputer.com/news/security/zero-click-ai-data-leak-flaw-uncovered-in-microsoft-365-copilot/)
  - What happened: a prompt-injection embedded in an email can lead Copilot to leak privileged internal data without user interaction (`CVE-2025-32711`).
  - Skills that would help: `LLM01` prompt injection; `LLM06` sensitive disclosure mitigations; scoping what data the model can access; output post-processing to block risky content.
3. [Claudy Day: chaining prompt injection and data exfiltration in Claude.ai (Oasis Security)](https://www.oasis.security/blog/claude-ai-prompt-injection-data-exfiltration-vulnerability)
  - What happened: multiple issues chain into a pipeline that manipulates instructions and exfiltrates conversation history/memory.
  - Skills that would help: prompt injection design (`LLM01`); treat model input/output as untrusted; governance/integration permission minimisation.
4. [AI chat app leak exposes 300 million messages tied to 25 million users (Malwarebytes)](https://www.malwarebytes.com/blog/news/2026/02/ai-chat-app-leak-exposes-300-million-messages-tied-to-25-million-users)
  - What happened: a misconfigured backend (Firebase security rules) exposed large volumes of chat messages and related user files.
  - Skills that would help: “assume logs/prompts can be exposed”; protect PII; secure the storage/access layer; monitoring/incident response if leakage is found.
5. [AI chatbot startup WotNot leaks 346,000 files, including passports and medical records (Bitdefender)](https://www.bitdefender.com/en-gb/blog/hotforsecurity/ai-chatbot-startup-wotnot-leaks-346-000-files-including-passports-and-medical-records)
  - What happened: sensitive files were left accessible via misconfigured cloud storage.
  - Skills that would help: least-privilege and secure storage; treat “AI app” as a normal system with real data protections; detection + containment when exposure is discovered.
6. [New attack on ChatGPT research agent pilfers secrets from Gmail inboxes (Ars Technica)](https://arstechnica.com/information-technology/2025/09/new-attack-on-chatgpt-research-agent-pilfers-secrets-from-gmail-inboxes/)
  - What happened: prompt injection in emails can cause an agent with inbox access to exfiltrate data without interactive clicks (no obvious “user action”).
  - Skills that would help: producer-side threat modelling (`LLM01` + `LLM08` excess agency); scope tools/permissions; human-in-the-loop for critical actions; monitor tool calls.
7. [GitHub Copilot Chat turns blabbermouth with crafty prompt injection attack (CamoLeak) (The Register)](https://www.theregister.com/2025/10/09/github_copilot_chat_vulnerability/)
  - What happened: hidden prompt injection via markdown/comment channels can trick Copilot Chat into exfiltrating secrets and private code.
  - Skills that would help: treat Copilot output like an untrusted PR (`LLM02` + `LLM09`); avoid accepting “security-sensitive” suggestions without verification/testing; mitigate risky context flows.

## Secure DevOps / supply chain stories (Module 7)

1. [GitHub repo artifacts leak tokens (ArtiPACKED) (Unit 42)](https://unit42.paloaltonetworks.com/github-repo-artifacts-leak-tokens)
  - What happened: GitHub Actions artifacts/race conditions can expose auth tokens from CI/CD workflows.
  - Skills that would help: pipeline hardening and policy; reduce secrets exposure in artifacts; artifact integrity/signing/attestation; detect and respond with fast revocation when leaked.
2. [Lottie Player compromised in supply chain attack - all you need to know (Sonatype)](https://www.sonatype.com/blog/lottie-player-compromised-in-supply-chain-attack-all-you-need-to-know)
  - What happened: a popular npm package was compromised and distributed malicious versions that targeted user crypto wallets.
  - Skills that would help: supply chain controls (`SCA`, SBOM/inventory, signing/attestation, integrity verification); “block when risk is clear” policy; triage + incident runbooks.
3. [New PhantomRaven NPM attack wave steals dev data via 88 packages (BleepingComputer)](https://www.bleepingcomputer.com/news/security/new-phantomraven-npm-attack-wave-steals-dev-data-via-88-packages/)
  - What happened: malicious npm packages in a slopsquatting campaign exfiltrate developer secrets and CI/CD tokens.
  - Skills that would help: treat dependencies as code you did not write; SCA + baseline controls; verify build inputs; incident response readiness when compromise is suspected.

## App security + web/session stories (Modules 3–6)

1. [WP Automatic WordPress plugin hit by millions of SQL injection attacks (BleepingComputer)](https://www.bleepingcomputer.com/news/security/wp-automatic-wordpress-plugin-hit-by-millions-of-sql-injection-attacks/)
  - What happened: SQL injection in plugin auth/queries enabled attackers to create admin users and plant backdoors.
    - Skills that would help: `SQLi` prevention via parameterised queries; input validation; secure coding review.
2. [Exploit for critical Fortra FileCatalyst Workflow SQLi flaw released (BleepingComputer)](https://www.bleepingcomputer.com/news/security/exploit-for-critical-fortra-filecatalyst-workflow-sqli-flaw-released/)
  - What happened: unauthenticated SQL injection allowed creation of rogue admin users and database manipulation (public PoC/exploit).
    - Skills that would help: server-side input handling; parameterised DB access; threat modelling for unauthenticated endpoints; patch fast.
3. [Critical flaw in LayerSlider WordPress plugin impacts 1 million sites (BleepingComputer)](https://www.bleepingcomputer.com/news/security/critical-flaw-in-layerslider-wordpress-plugin-impacts-1-million-sites/)
  - What happened: unauthenticated SQL injection (time-based blind) could extract sensitive data like password hashes.
    - Skills that would help: `SQLi` secure coding; safe query construction; testing + WAF-friendly logging; keep plugins updated.
4. [New attack uses MSC files and Windows XSS flaw to breach networks (BleepingComputer)](https://www.bleepingcomputer.com/news/security/new-grimresource-attack-uses-msc-files-and-windows-xss-flaw-to-breach-networks/)
  - What happened: a Windows DOM-based XSS flaw was abused as part of “GrimResource” to execute code for initial network access.
    - Skills that would help: XSS prevention/defence-in-depth; treat client-side injection as a stepping stone; harden endpoints and patch.
5. [Over 1 Million websites are at risk of sensitive information leakage — XSS is dead. Long live XSS (Salt Labs)](https://salt.security/blog/over-1-million-websites-are-at-risk-of-sensitive-information-leakage---xss-is-dead-long-live-xss)
  - What happened: shows how XSS can combine with OAuth flows (Hotjar example) to leak sensitive OAuth/OAuth-adjacent data.
    - Skills that would help: XSS secure coding + output encoding; session/cookie protection; assume OAuth tokens/redirect flows are sensitive.
6. [LeakyCLI: AWS & Google Cloud command-line tools can expose sensitive credentials in build logs (Orca Security)](https://orca.security/resources/blog/leakycli-aws-google-cloud-command-line-tools-can-expose-sensitive-credentials-build-logs/)
  - What happened: CLI tools used in CI can unintentionally expose credentials/secrets in build logs.
    - Skills that would help: pipeline insecurity controls (avoid leaking secrets in logs/artifacts); secrets management; least privilege; audit logs.
7. [Stealing HttpOnly cookies with the cookie sandwich technique (PortSwigger Research)](https://portswigger.net/research/stealing-httponly-cookies-with-the-cookie-sandwich-technique)
  - What happened: “cookie sandwich” technique bypasses HttpOnly under certain conditions to steal session cookies.
    - Skills that would help: session cookie hardening; correct cookie parsing/encoding; XSS/response reflection avoidance; defence-in-depth around cookies.

## C#/.NET stories (Modules 4–6 / 5)

1. [Microsoft fixes highest-severity ASP.NET Core flaw ever (CVE-2025-55315)](https://www.bleepingcomputer.com/news/microsoft/microsoft-fixes-highest-severity-aspnet-core-flaw-ever/)
  - What happened: HTTP request smuggling in Kestrel allowed attackers to bypass front-end security checks (auth/CSRF) and hijack credentials in worst cases.
    - Skills that would help: consistent input handling and validation at every layer; secure defaults; test request parsing behavior; incident readiness to respond quickly.
2. [Code injection attacks using publicly disclosed ASP.NET machine keys (Microsoft Security Blog)](https://www.microsoft.com/en-us/security/blog/2025/02/06/code-injection-attacks-using-publicly-disclosed-asp-net-machine-keys/)
  - What happened: publicly available ASP.NET `machineKey` values were reused to craft ViewState payloads, enabling code injection and RCE.
    - Skills that would help: secret/key management (no hardcoded/default keys); protect and rotate auth/crypto material; harden middleware/web.config; verify fixes with security testing.
3. [Sitecore CMS exploit chain starts with hardcoded 'b' password (BleepingComputer)](https://www.bleepingcomputer.com/news/security/sitecore-cms-exploit-chain-starts-with-hardcoded-b-password/)
  - What happened: a pre-auth RCE chain hinged on a hardcoded internal password, then chained into other issues to reach a webshell/RCE path.
    - Skills that would help: secure secret handling; strict input validation (including path handling for uploads); least privilege + authorization checks; regression tests to catch exploit chains.

