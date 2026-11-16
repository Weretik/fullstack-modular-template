# 0003. A React single-page app for the admin, never indexed

Status: accepted, 2026-11-16

## Context

The admin app is used by signed-in staff only. It needs rich forms and tables, not server rendering or SEO.

## Decision

Build the admin as a React single-page app with Vite, Material UI and React Query for server state. It sends
`noindex` in its HTML and disallows everything in `robots.txt`. It never talks to the storefront.

## Consequences

- Simple static hosting; no server runtime for the admin.
- Server data is cached, refetched and invalidated by React Query instead of hand-written state.
- Search engines never index staff screens.
