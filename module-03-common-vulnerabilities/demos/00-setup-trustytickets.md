# Demo 00: TrustyTickets setup (and login cookies)

This demo gets the lab app running and gives you copy/paste commands to log in as test users.

## Safety reminder (say this out loud)

- TrustyTickets is **deliberately vulnerable**.
- Run **locally** or in a dedicated lab environment.
- Never use real credentials or data.

---

## 1) Start the app

From the repo root:

```bash
cd TrustyTickets
dotnet run
```

Open the URL shown (typically `http://localhost:5000`).

Default users:

- `alice` / `alice123`
- `bob` / `bob456`
- `admin` / `admin`

---

## 2) Set a base URL (optional)

If you prefer to run on a specific port:

```bash
dotnet run --urls http://localhost:5005
```

In the commands below, we assume:

```bash
export TT_BASE="http://localhost:5005"
```

---

## 3) Log in and capture cookies (for curl demos)

Create cookie jars in the current folder.

```bash
curl -s -c alice.cookies -H "Content-Type: application/json" \
  -d '{"userName":"alice","password":"alice123"}' \
  "$TT_BASE/api/auth/login" > /dev/null

curl -s -c bob.cookies -H "Content-Type: application/json" \
  -d '{"userName":"bob","password":"bob456"}' \
  "$TT_BASE/api/auth/login" > /dev/null

curl -s -c admin.cookies -H "Content-Type: application/json" \
  -d '{"userName":"admin","password":"admin"}' \
  "$TT_BASE/api/auth/login" > /dev/null
```

Quick sanity check:

```bash
curl -s -b alice.cookies "$TT_BASE/api/auth/me"
```

Expected: JSON describing Alice.

---

## Talking points

- The app uses a simple cookie named `Session` that stores a user id.
- That’s intentionally simplified so we can focus on vulnerability patterns.

