# 0009. Cookie session for the admin app

Status: accepted, 2026-11-16

## Context

The admin app runs in a browser. Tokens stored in JavaScript-readable storage can be stolen by any injected
script, and refresh-token handling adds code on both sides.

## Decision

Staff sign in with ASP.NET Core Identity and receive an HttpOnly, Secure, `SameSite=Strict` cookie. The API
answers 401/403 instead of redirecting. In development the admin dev server proxies `/api`, so the cookie stays on
one origin.

## Consequences

- Scripts cannot read the session; `SameSite=Strict` blocks cross-site request forgery.
- The admin app and the API must share a site in production (for example `admin.example.com` and
  `api.example.com`).
- A native mobile client would need bearer tokens as a separate, additional scheme.
