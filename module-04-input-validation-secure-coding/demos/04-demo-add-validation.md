# Demo 04: Add basic validation (robustness without “security theatre”)

Goal: show how validation improves correctness and reduces accidental 500s, while being explicit that validation is not a replacement for parameterisation/encoding.

## Suggested target

`POST /api/tickets` in `TrustyTickets/Controllers/TicketsController.cs`:

- It accepts `Title` and `Description`
- It’s easy to demonstrate a bad request and a better 400 response

## Validation rules (simple, teachable)

- Title:
  - required
  - length 3–80
- Description:
  - optional
  - max length 2000

## Example validation logic (conceptual)

In `TicketsController.Create`:

```csharp
if (string.IsNullOrWhiteSpace(req.Title))
    return BadRequest(new { error = "Title is required." });

if (req.Title.Length < 3 || req.Title.Length > 80)
    return BadRequest(new { error = "Title must be 3–80 characters." });

if (req.Description != null && req.Description.Length > 2000)
    return BadRequest(new { error = "Description must be <= 2000 characters." });
```

## Demo flow

1. Show the current behaviour with an empty title (or huge payload).
2. Add validation.
3. Repeat the same request and show a clean 400 with a clear message.

## Talking points

- Validation is about **allowed inputs** (type/length/format/business rules).
- Validation does not “fix” SQL injection or XSS by itself.
- It makes APIs predictable and reduces accidental failure modes and logging noise.

