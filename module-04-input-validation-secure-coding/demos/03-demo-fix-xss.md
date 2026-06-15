# Demo 03: Fix stored XSS (safe rendering) + re-test payload

Goal: show that fixing XSS is mostly about **changing rendering sinks**, not about clever “cleaning”.

## What you’ll change

### File

- Vulnerable UI: `TrustyTickets/wwwroot/ticket.html`

### Vulnerable functions (current)

- Comments: `renderComment` uses `div.innerHTML = ... ${c.body}`
- Description: `setDescription` uses `wrap.innerHTML = html`

These are intentionally unsafe sinks.

## Fix approach (recommended for the demo)

### Comments: render safely

Replace “build HTML string” with DOM creation and `textContent`.

Conceptual replacement:

```javascript
function renderComment(c) {
  const div = document.createElement('div');
  div.className = 'comment';

  const author = document.createElement('span');
  author.className = 'author';
  author.textContent = (c.author?.displayName || '');

  const sep = document.createTextNode(': ');
  const body = document.createElement('span');
  body.textContent = (c.body || '');

  div.appendChild(author);
  div.appendChild(sep);
  div.appendChild(body);
  return div;
}
```

### Description: treat as text (or sanitise)

For the demo, treat description as plain text:

```javascript
function setDescription(text) {
  const wrap = document.createElement('div');
  wrap.className = 'description';
  wrap.textContent = text || '(no description)';
  return wrap;
}
```

> **Note:** If your requirements call for allowing *some* HTML formatting in ticket descriptions or comments (for example, bold text, links, or lists), you must not simply allow raw HTML or use `innerHTML` with user-provided input. Doing so would reintroduce XSS risks.

To safely support limited HTML, you should:

- **Use a strict sanitiser allowlist:** Only permit a short, known-safe set of HTML tags and attributes (e.g., `<b>`, `<i>`, `<a href="">`), stripping or escaping all else. There are libraries available for this purpose:
  - In JavaScript, options include [DOMPurify](https://github.com/cure53/DOMPurify) (browser/Node), [sanitize-html](https://github.com/apostrophecms/sanitize-html), etc.
  - Configure the sanitiser strictly—do not use the default “allow everything” mode.
- **Markdown as an alternative:** Convert Markdown to HTML *and* sanitise the output. Many libraries provide this pipeline: render Markdown, then clean the result with a sanitiser as above.
- **Never trust user-provided HTML by default**, even if it “looks safe.”

**Example using DOMPurify:**
```javascript
function setDescription(html) {
  const wrap = document.createElement('div');
  wrap.className = 'description';
  // Only use DOMPurify.sanitize on trusted/expected HTML content
  wrap.innerHTML = DOMPurify.sanitize(html, {ALLOWED_TAGS: ['b', 'i', 'a'], ALLOWED_ATTR: ['href']});
  return wrap;
}
```
> Always evaluate the risk for your context. Allowing HTML is a major expansion of attack surface and should only be supported with strong sanitisation and a clear allowlist policy.


## Prove it’s fixed (rerun the same payload)

### Before (baseline)

Paste into comment box:

```html
<img src=x onerror=alert(1)>
```

Expected: alert / attacker-controlled behaviour.

### After

Repeat the same input.

Expected:

- The payload appears as literal text (or as a harmless string)
- No execution

## Talking points

- XSS is prevented by **encoding or safe rendering**.
- Frameworks that auto-escape make this easy—until someone uses a “raw”/`innerHTML` escape hatch.
- CSP is a useful backstop, but safe rendering is the primary fix.

