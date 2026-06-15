/**
 * Vanilla JS API helpers. Use credentials: 'include' for cookie auth.
 * Deliberately no CSRF token for teaching; add when fixing.
 */
const API_BASE = '';

async function api(path, options = {}) {
  const res = await fetch(API_BASE + path, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
    credentials: 'include',
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.error || res.statusText || `HTTP ${res.status}`);
  }
  if (res.status === 204) return null;
  return res.json();
}

function redirectIfNotLoggedIn(me) {
  if (!me) {
    window.location.href = '/login.html';
    return true;
  }
  return false;
}
