# 0004. A typed API client generated from the backend contract

Status: accepted, 2026-11-16

## Context

Hand-written request types drift from the API and break at runtime instead of at build time.

## Decision

Generate TypeScript types from the backend's `openapi.json` with `openapi-typescript` into
`libs/api-client`, and call the API only through `openapi-fetch` clients created there. The generated file
is committed; CI regenerates it and fails when it differs. Apps never use raw `fetch` or Axios for the API.

## Consequences

- A backend contract change breaks the frontend build where it matters.
- Paths, parameters and bodies are checked by the compiler.
- Regenerate the client after every backend contract change (`npm run contracts:generate`).
