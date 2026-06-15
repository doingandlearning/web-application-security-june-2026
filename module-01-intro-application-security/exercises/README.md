# Module 1 — Exercises

## Lab 1: Map Your Attack Surface

---

## Objective

Map the attack surface and trust boundaries of a simple system. You will practice identifying untrusted inputs, trust boundaries, and sensitive assets — the mental model you will reuse throughout the course.

You will:

1. Sketch your system and identify external actors and entry points.
2. Mark trust boundaries where untrusted data crosses into trusted components.
3. Contribute attack surface and "what could go wrong?" ideas to a shared board.
4. Pick the point you think carries the highest risk.

---

## Scenario: TrustyTickets (or your app)

You are part of a team building a web-based ticketing system called TrustyTickets. The system:

- Lets users sign up, log in, and create tickets.
- Allows users to view and comment on their own tickets.
- Exposes an API that a mobile app and partner systems can call.
- Stores tickets and comments in a database.

This maps to most CRUD-style business apps — some UI, some APIs, a database, and integrations. Use TrustyTickets as the default, or substitute your own app if that makes the discussion more relevant.

---

## Step 1: Individual sketch (3–5 minutes)

On paper or in a drawing tool, sketch the main components and how data flows between them.

**Your task:**

Draw a simple diagram with:
- External users — browser, mobile app, partner system.
- Frontend — web app or mobile app.
- Backend API.
- Database and at least one external service — email, payment, logging, or a third-party API.

Add arrows showing data flow. You will not share this yet — it is to organise your own thinking before the shared activity.

**Hints:**
- Keep it to 5–8 boxes; this is a conversation tool, not a formal diagram.
- Ask "what talks to what, and over which channel?" — browser → HTTPS → API, API → DB, and so on.
- If you are using your real system, focus on one slice — for example, "user logs in and creates a ticket".

<details>
<summary>Possible solution</summary>

```
[User Browser] --HTTPS--> [Web App] --HTTPS--> [Backend API] --SQL--> [Database]

[Mobile App] --HTTPS--> [Backend API]

[Partner System] --HTTPS (partner key)--> [Backend API]

[Backend API] --SMTP/HTTP--> [Email Service / Third-party API]
```

</details>

---

## Step 2: Shared board — attack surface and trust boundaries (5–10 minutes)

The facilitator will open a shared whiteboard (Teams Whiteboard, Miro, or Excalidraw) with a skeleton diagram and two areas: **Attack surface** and **What could go wrong?**

**Your task:**

Add 1–2 sticky notes to each area:

- **Attack surface** — a specific place where an attacker could send data or influence behaviour. Examples: login form, comment API, file upload, partner webhook, admin endpoint.
- **What could go wrong?** — one sentence describing what an attacker might try and what the impact could be.

Keep each note to a single idea so it is easy to group and discuss.

**Hints:**
- Think about both browser requests and direct API calls.
- Attack surface = anywhere an attacker can send data or influence behaviour.
- Trust boundaries = where you cross from "we don't control this" to "we trust this more" — mark at least two.

---

## Step 3: Plenary debrief and prioritisation (10–15 minutes)

**Your task:**

Type in chat: **"Which point feels highest-risk in your system?"**

For example: `login`, `comments`, `partner API`, or `other — [short note]`.

The facilitator will group the board notes into themes — injection, broken access control, auth and session, data exposure — and connect them to later modules.

If time allows, 1–2 volunteers share their individual sketch via screen share to compare with the shared board.

---

## Expected output

- Individual sketches retained by each participant.
- A shared whiteboard with attack surface notes and "what could go wrong?" notes grouped by theme.
- A chat poll result showing which points feel highest-risk across the group's real systems.

---

## Key concepts

- **Attack surface** — every point where an attacker can send data or influence behaviour.
- **Trust boundaries** — where data moves from untrusted to trusted, and where you validate and enforce policy.
- **Threat brainstorming** — turning a system sketch into concrete "what could go wrong?" hypotheses.
- **Prioritisation** — picking the highest-risk area rather than treating all inputs as equal.

---

## Next steps

You will reuse this map in Module 2 (threat modelling) and Module 3 (hands-on exploitation). Keep your diagram — you will refine it as the course progresses.