# Demo 00: Setup + find the session cookie (TrustyTickets)

Goal: get everyone looking at the same things: where login happens, what cookie is set, and where in code it’s configured.

## Run the app

```bash
cd TrustyTickets
dotnet run
```

Open the URL shown (e.g. `http://localhost:5000`).

Default users:

- `alice` / `alice123`
- `bob` / `bob456`
- `admin` / `admin`

## Where the cookie is set

- Login endpoint: `POST /api/auth/login`
- Code: `TrustyTickets/Controllers/AuthController.cs`
  - Look for `Response.Cookies.Append("Session", ...)`

## What to show in the browser

1. Log in as `alice`
2. Open DevTools → Application/Storage → Cookies
3. Find cookie named `Session`

Talking points:

- This app intentionally uses a simplistic cookie “session” (user id in cookie) so the course can focus on security concepts.
- In production you’d typically store a random session id and keep session state server-side.

